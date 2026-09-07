using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Memoria.Launcher.Controller;

namespace Memoria.Launcher.Utils.ModValidation
{
    public partial class ModValidationWindow : Window
    {
        private const Int32 ScreenMargin = 50;
        private const UInt32 MonitorDefaultToNearest = 2;
        private const UInt32 SetWindowPosNoActivate = 0x0010;
        private const UInt32 SetWindowPosNoZOrder = 0x0004;

        private readonly String _rootDirectory;
        private readonly String _rootName;
        private readonly ModValidationService _service;
        private readonly CancellationTokenSource _cancellation = new();
        private readonly Dictionary<String, ModValidationResult> _results = new(StringComparer.OrdinalIgnoreCase);
        private readonly IDisposable _gamepadNavigation;
        private ModValidationReport _report;
        private ValidationDisplayNode _selectedNode;
        private Boolean _running;
        private Boolean _closed;

        internal ModValidationWindow(String rootDirectory, String rootName, ModValidationService service)
        {
            _rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));
            _rootName = rootName ?? throw new ArgumentNullException(nameof(rootName));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            InitializeComponent();
            Title = $"Validate mod — {_rootName}";
            ShowAllFilesCheckBox.Checked += OnViewChanged;
            ShowAllFilesCheckBox.Unchecked += OnViewChanged;
            TreeViewCheckBox.Checked += OnViewChanged;
            TreeViewCheckBox.Unchecked += OnViewChanged;
            SourceInitialized += OnSourceInitialized;
            Loaded += OnLoaded;
            Closing += OnClosing;
            Closed += OnClosed;
            GamepadNavigation.SetIsModalScope((DependencyObject)Content, true);
            GamepadNavigation.SetIsCancelAction(CloseButton, true);
            _gamepadNavigation = GamepadNavigationService.Attach(this);
        }

        private async void OnLoaded(Object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            await RunValidationAsync();
        }

        private async Task RunValidationAsync()
        {
            _running = true;
            CloseButton.Content = "Cancel";
            ValidationProgressBar.IsIndeterminate = true;
            SummaryTextBlock.Text = $"Using {Environment.ProcessorCount} validation workers";
            Progress<ModValidationProgress> progress = new(OnProgress);
            try
            {
                _report = await _service.ValidateDirectoryAsync(_rootDirectory, _rootName, progress, _cancellation.Token);
                if (_closed)
                    return;
                foreach (ModValidationResult result in _report.FlattenFiles())
                    _results[result.File.FullPath] = result;
                RebuildViews(null);
                Int32 problems = _report.FlattenFiles().Count(result => result.HasProblem);
                CurrentStatusTextBlock.Text = problems == 0 ? "Validation completed. No problems found." : $"Validation completed with {problems} problem file{(problems == 1 ? String.Empty : "s")}.";
                SummaryTextBlock.Text = BuildSummary(_report.FlattenFiles());
                ValidationProgressBar.IsIndeterminate = false;
                ValidationProgressBar.Value = ValidationProgressBar.Maximum;
            }
            catch (OperationCanceledException)
            {
                if (!_closed)
                    CurrentStatusTextBlock.Text = "Validation cancelled.";
            }
            catch (Exception exception)
            {
                if (!_closed)
                {
                    CurrentStatusTextBlock.Text = "Validation could not be completed.";
                    DiagnosticsTextBox.Text = exception.ToString();
                }
            }
            finally
            {
                _running = false;
                if (!_closed)
                {
                    CloseButton.Content = "Close";
                    RevalidateButton.IsEnabled = _selectedNode != null && _service.CanValidate(_selectedNode.Result.File);
                }
            }
        }

        private void OnProgress(ModValidationProgress progress)
        {
            if (_closed)
                return;
            if (progress.CompletedResult != null)
                _results[progress.CompletedResult.File.FullPath] = progress.CompletedResult;

            if (progress.EnumerationCompleted)
            {
                ValidationProgressBar.IsIndeterminate = false;
                ValidationProgressBar.Maximum = Math.Max(1, progress.DiscoveredFiles);
                ValidationProgressBar.Value = Math.Min(progress.CompletedFiles, progress.DiscoveredFiles);
                CurrentStatusTextBlock.Text = String.IsNullOrEmpty(progress.CurrentFile) ? $"Validating files: {progress.CompletedFiles} / {progress.DiscoveredFiles}" : $"Validating {progress.CurrentFile} — {progress.CompletedFiles} / {progress.DiscoveredFiles}";
            }
            else
            {
                ValidationProgressBar.IsIndeterminate = true;
                CurrentStatusTextBlock.Text = $"Discovering files ({progress.DiscoveredFiles}); processing {progress.CurrentFile}";
            }
        }

        private void OnViewChanged(Object sender, RoutedEventArgs e)
        {
            if (_report != null)
                RebuildViews(_selectedNode?.RelativePath);
        }

        private void RebuildViews(String selectedPath)
        {
            Boolean showAll = ShowAllFilesCheckBox.IsChecked == true;
            ValidationDisplayNode root = CreateDisplayNode(_report.Tree, showAll);
            List<ValidationDisplayNode> flatFiles = FlattenDisplayNodes(_report.Tree).Where(node => showAll || node.Result.HasProblem).ToList();
            if (!showAll && flatFiles.Count == 0)
            {
                ValidationDisplayNode successMessage = ValidationDisplayNode.CreateSuccessMessage();
                ResultsTreeView.ItemsSource = new[] { successMessage };
                ResultsListBox.ItemsSource = new[] { successMessage };
                root = successMessage;
            }
            else
            {
                ResultsTreeView.ItemsSource = root == null ? null : new[] { root };
                ResultsListBox.ItemsSource = flatFiles;
            }
            Boolean treeView = TreeViewCheckBox.IsChecked == true;
            ResultsTreeView.Visibility = treeView ? Visibility.Visible : Visibility.Collapsed;
            ResultsListBox.Visibility = treeView ? Visibility.Collapsed : Visibility.Visible;
            RestoreSelection(selectedPath, root);
        }

        private static ValidationDisplayNode CreateDisplayNode(ModValidationTreeNode node, Boolean showAll)
        {
            List<ValidationDisplayNode> children = node.Children.Select(child => CreateDisplayNode(child, showAll)).Where(child => child != null).ToList();
            if (node.IsFile && (showAll || node.Result.HasProblem))
                return new ValidationDisplayNode(node, children);
            if (!node.IsFile && children.Count > 0)
                return new ValidationDisplayNode(node, children);
            return null;
        }

        private static IEnumerable<ValidationDisplayNode> FlattenDisplayNodes(ModValidationTreeNode node)
        {
            if (node.IsFile)
                yield return new ValidationDisplayNode(node, Array.Empty<ValidationDisplayNode>());
            foreach (ModValidationTreeNode child in node.Children)
                foreach (ValidationDisplayNode file in FlattenDisplayNodes(child))
                    yield return file;
        }

        private void RestoreSelection(String relativePath, ValidationDisplayNode root)
        {
            if (String.IsNullOrEmpty(relativePath))
            {
                SelectNode(null);
                return;
            }

            ValidationDisplayNode node = FindNode(root, relativePath);
            if (node == null)
                node = ResultsListBox.Items.Cast<ValidationDisplayNode>().FirstOrDefault(item => String.Equals(item.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase));
            SelectNode(node);
        }

        private static ValidationDisplayNode FindNode(ValidationDisplayNode root, String relativePath)
        {
            if (root == null)
                return null;
            if (String.Equals(root.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase))
                return root;
            foreach (ValidationDisplayNode child in root.Children)
            {
                ValidationDisplayNode match = FindNode(child, relativePath);
                if (match != null)
                    return match;
            }
            return null;
        }

        private void OnTreeSelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e) => SelectNode(e.NewValue as ValidationDisplayNode);

        private void OnListSelectionChanged(Object sender, SelectionChangedEventArgs e) => SelectNode(ResultsListBox.SelectedItem as ValidationDisplayNode);

        private void OnListMouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            ModValidationResult result = (ResultsListBox.SelectedItem as ValidationDisplayNode)?.Result;
            if (result == null)
                return;

            try
            {
                if (!File.Exists(result.File.FullPath))
                    throw new FileNotFoundException("The selected file no longer exists.", result.File.FullPath);
                Process.Start(new ProcessStartInfo(result.File.FullPath) { UseShellExecute = true });
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, $"Unable to open the selected file.{Environment.NewLine}{Environment.NewLine}{exception.Message}", "Open file failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectNode(ValidationDisplayNode node)
        {
            _selectedNode = node?.Result == null ? null : node;
            RevalidateButton.IsEnabled = !_running && _selectedNode != null && _service.CanValidate(_selectedNode.Result.File);
            DiagnosticsTextBox.Text = _selectedNode == null ? String.Empty : FormatDiagnostics(_selectedNode.Result);
        }

        private async void OnRevalidateClick(Object sender, RoutedEventArgs e)
        {
            ModValidationResult selected = _selectedNode?.Result;
            if (selected == null || !_service.CanValidate(selected.File))
                return;

            String selectedPath = selected.File.RelativePath;
            _running = true;
            RevalidateButton.IsEnabled = false;
            CloseButton.Content = "Cancel";
            CurrentStatusTextBlock.Text = $"Validating {selectedPath}";
            try
            {
                ModValidationResult result = await _service.ValidateFileAsync(selected.File, _cancellation.Token);
                if (_closed)
                    return;
                _results[result.File.FullPath] = result;
                _report = new ModValidationReport(_rootName, _results.Values);
                RebuildViews(selectedPath);
                CurrentStatusTextBlock.Text = result.HasProblem ? $"{selectedPath} still has problems." : $"{selectedPath} passed validation.";
                SummaryTextBlock.Text = BuildSummary(_report.FlattenFiles());
            }
            catch (OperationCanceledException)
            {
                if (!_closed)
                    CurrentStatusTextBlock.Text = "Validation cancelled.";
            }
            finally
            {
                _running = false;
                if (!_closed)
                {
                    CloseButton.Content = "Close";
                    RevalidateButton.IsEnabled = _selectedNode != null && _service.CanValidate(_selectedNode.Result.File);
                }
            }
        }

        private static String FormatDiagnostics(ModValidationResult result)
        {
            StringBuilder text = new();
            text.AppendLine(result.File.RelativePath);
            text.AppendLine($"Status: {result.Status}");
            text.AppendLine($"Validator: {(String.IsNullOrEmpty(result.ValidatorName) ? "None" : result.ValidatorName)}");
            text.AppendLine();
            foreach (ModValidationDiagnostic diagnostic in result.Diagnostics)
            {
                text.AppendLine($"[{diagnostic.Severity}]");
                text.AppendLine(diagnostic.Message);
                text.AppendLine();
            }
            return text.ToString().TrimEnd();
        }

        private static String BuildSummary(IReadOnlyList<ModValidationResult> results)
        {
            Int32 valid = results.Count(result => result.Status == ModValidationStatus.Valid);
            Int32 problems = results.Count(result => result.HasProblem);
            Int32 skipped = results.Count(result => result.Status == ModValidationStatus.Skipped);
            return $"Files: {results.Count}   Valid: {valid}   Problems: {problems}   Skipped: {skipped}";
        }

        private void OnCloseClick(Object sender, RoutedEventArgs e) => Close();

        private void OnClosing(Object sender, CancelEventArgs e)
        {
            _closed = true;
            _cancellation.Cancel();
        }

        private void OnClosed(Object sender, EventArgs e)
        {
            _gamepadNavigation.Dispose();
            _cancellation.Dispose();
        }

        private void OnSourceInitialized(Object sender, EventArgs e)
        {
            IntPtr ownerHandle = Owner == null ? IntPtr.Zero : new WindowInteropHelper(Owner).Handle;
            IntPtr monitor = MonitorFromWindow(ownerHandle, MonitorDefaultToNearest);
            MonitorInfo info = MonitorInfo.Create();
            if (monitor == IntPtr.Zero || !GetMonitorInfo(monitor, ref info))
                return;

            Int32 width = Math.Max(1, info.WorkArea.Right - info.WorkArea.Left - ScreenMargin * 2);
            Int32 height = Math.Max(1, info.WorkArea.Bottom - info.WorkArea.Top - ScreenMargin * 2);
            SetWindowPos(new WindowInteropHelper(this).Handle, IntPtr.Zero, info.WorkArea.Left + ScreenMargin, info.WorkArea.Top + ScreenMargin, width, height, SetWindowPosNoActivate | SetWindowPosNoZOrder);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr window, UInt32 flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean SetWindowPos(IntPtr window, IntPtr insertAfter, Int32 x, Int32 y, Int32 width, Int32 height, UInt32 flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRectangle
        {
            public Int32 Left;
            public Int32 Top;
            public Int32 Right;
            public Int32 Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MonitorInfo
        {
            public Int32 Size;
            public NativeRectangle MonitorArea;
            public NativeRectangle WorkArea;
            public UInt32 Flags;

            public static MonitorInfo Create() => new() { Size = Marshal.SizeOf<MonitorInfo>() };
        }
    }
}
