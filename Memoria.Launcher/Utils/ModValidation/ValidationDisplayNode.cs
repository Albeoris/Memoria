using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Memoria.Launcher.Utils.ModValidation
{
    public sealed class ValidationDisplayNode
    {
        internal ValidationDisplayNode(ModValidationTreeNode node, IReadOnlyList<ValidationDisplayNode> children)
        {
            Node = node;
            Name = node.Name;
            RelativePath = node.RelativePath;
            Children = children;
            if (!node.IsFile)
            {
                Icon = "📁";
                IconBrush = Brushes.LightGray;
            }
            else if (node.Result.Status == ModValidationStatus.Valid || node.Result.Status == ModValidationStatus.Fixed)
            {
                Icon = "✅";
                IconBrush = Brushes.LimeGreen;
            }
            else if (node.Result.HasProblem)
            {
                Icon = "❌";
                IconBrush = Brushes.IndianRed;
            }
            else
            {
                Icon = "▪";
                IconBrush = Brushes.Gray;
            }
        }

        private ValidationDisplayNode(String successMessage)
        {
            Name = successMessage;
            RelativePath = successMessage;
            Icon = String.Empty;
            IconBrush = Brushes.Transparent;
            Children = Array.Empty<ValidationDisplayNode>();
            IsSuccessMessage = true;
        }

        internal ModValidationTreeNode Node { get; }
        internal ModValidationResult Result => Node?.Result;
        public String Name { get; }
        public String RelativePath { get; }
        public String Icon { get; }
        public Brush IconBrush { get; }
        public IReadOnlyList<ValidationDisplayNode> Children { get; }
        public Boolean IsSuccessMessage { get; }

        internal static ValidationDisplayNode CreateSuccessMessage() => new("All files passed validation.");
    }
}
