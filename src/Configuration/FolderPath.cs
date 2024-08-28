namespace Configuration;

public class FolderPath
{
    public bool IsRooted => !string.IsNullOrWhiteSpace(Root);
    public string Root { get; private set; }
    public string[] Folders { get; private set; }

    public FolderPath()
    {
    }

    public FolderPath With(string root = null, IEnumerable<string> folders = null)
    {
        return new FolderPath
        {
            Root = root ?? Root,
            Folders = (folders ?? Folders).ToArray()
        };
    }

    public static FolderPath Parse(string path)
    {
        var root = Path.IsPathRooted(path) ? Path.GetPathRoot(path) : null;
        var relativePath = root == null ? path : path.Substring(root.Length);
        var folders = relativePath.Split(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar},
            StringSplitOptions.RemoveEmptyEntries);
        return new FolderPath
        {
            Root = root,
            Folders = folders
        };
    }

    public override string ToString()
    {
        return IsRooted
            ? $"{Root}{Path.Combine(Folders)}"
            : Path.Combine(Folders);
    }

    public static implicit operator string(FolderPath path)
    {
        return path?.ToString();
    }
}