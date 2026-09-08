using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModValidationTreeNode
    {
        private readonly List<ModValidationTreeNode> _children = new();

        private ModValidationTreeNode(String name, String relativePath, ModValidationResult result)
        {
            Name = name;
            RelativePath = relativePath;
            Result = result;
        }

        public String Name { get; }
        public String RelativePath { get; }
        public ModValidationResult Result { get; private set; }
        public Boolean IsFile => Result != null;
        public IReadOnlyList<ModValidationTreeNode> Children => _children;

        public static ModValidationTreeNode Create(String rootName, IEnumerable<ModValidationResult> results)
        {
            ModValidationTreeNode root = new(rootName ?? throw new ArgumentNullException(nameof(rootName)), String.Empty, null);
            foreach (ModValidationResult result in (results ?? throw new ArgumentNullException(nameof(results))).OrderBy(item => item.File.RelativePath, StringComparer.OrdinalIgnoreCase))
                root.Add(result);
            root.SortRecursively();
            return root;
        }

        public IReadOnlyList<ModValidationResult> FlattenFiles()
        {
            List<ModValidationResult> files = new();
            AddFilesTo(files);
            return files;
        }

        private void Add(ModValidationResult result)
        {
            String[] parts = result.File.RelativePath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            ModValidationTreeNode parent = this;
            String relativePath = String.Empty;
            for (Int32 index = 0; index < parts.Length; index++)
            {
                relativePath = String.IsNullOrEmpty(relativePath) ? parts[index] : Path.Combine(relativePath, parts[index]);
                Boolean isFile = index == parts.Length - 1;
                ModValidationTreeNode child = parent._children.FirstOrDefault(item => String.Equals(item.Name, parts[index], StringComparison.OrdinalIgnoreCase));
                if (child == null)
                {
                    child = new ModValidationTreeNode(parts[index], relativePath, isFile ? result : null);
                    parent._children.Add(child);
                }
                else if (isFile)
                {
                    child.Result = result;
                }
                parent = child;
            }
        }

        private void AddFilesTo(List<ModValidationResult> files)
        {
            if (Result != null)
                files.Add(Result);
            foreach (ModValidationTreeNode child in _children)
                child.AddFilesTo(files);
        }

        private void SortRecursively()
        {
            _children.Sort((left, right) =>
            {
                if (left.IsFile != right.IsFile)
                    return left.IsFile ? 1 : -1;
                return StringComparer.OrdinalIgnoreCase.Compare(left.Name, right.Name);
            });
            foreach (ModValidationTreeNode child in _children)
                child.SortRecursively();
        }
    }
}