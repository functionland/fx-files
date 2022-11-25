using Functionland.FxFiles.Client.App.Implementations;
using System.Text.RegularExpressions;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class FileViewer
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnPin { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnUnpin { get; set; }
    [Parameter] public EventCallback<FsArtifact> OnOptionClick { get; set; }
    [Parameter] public EventCallback<string> NavigationFolderCallback { get; set; }
    [AutoInject] public INativeNavigation NativeNavigation { get; set; } = default!;

    public bool IsModalOpen { get; set; } = false;

    private FsArtifact? _currentArtifact;

    private static readonly string[] textViewerSupportTypes = { ".dyalog", ".apl", ".asc", ".pgp", ".sig", ".asn", ".asn1", ".b", ".bf", ".c", ".h", ".ino", ".cpp", ".c++", ".cc", ".cxx", ".hpp", ".h++", ".hh", ".hxx", ".cob", ".cpy", ".cbl", ".cs", ".clj", ".cljc", ".cljx", ".cljs", ".gss", ".cmake", ".cmake.in", ".coffee", ".cl", ".lisp", ".el", ".cyp", ".cypher", ".pyx", ".pxd", ".pxi", ".cr", ".css", ".cql", ".d", ".dart", ".diff", ".patch", ".dtd", ".dylan", ".dyl", ".intr", ".ecl", ".edn", ".e", ".elm", ".ejs", ".erb", ".erl", ".factor", ".forth", ".fth", ".4th", ".f", ".for", ".f77", ".f90", ".f95", ".fs", ".s", ".feature", ".go", ".groovy", ".gradle", ".haml", ".hs", ".lhs", ".hx", ".hxml", ".aspx", ".html", ".htm", ".handlebars", ".hbs", ".pro", ".jade", ".pug", ".java", ".jsp", ".js", ".json", ".map", ".jsonld", ".jsx", ".j2", ".jinja", ".jinja2", ".jl", ".kt", ".less", ".ls", ".lua", ".markdown", ".md", ".mkd", ".m", ".nb", ".wl", ".wls", ".mo", ".mps", ".mbox", ".nsh", ".nsi", ".nt", ".nq", ".m", ".mm", ".ml", ".mli", ".mll", ".mly", ".m", ".oz", ".p", ".pas", ".jsonld", ".pl", ".pm", ".php", ".php3", ".php4", ".php5", ".php7", ".phtml", ".pig", ".txt", ".text", ".conf", ".def", ".list", ".log", ".pls", ".ps1", ".psd1", ".psm1", ".properties", ".ini", ".in", ".proto", ".BUILD", ".bzl", ".py", ".pyw", ".pp", ".q", ".r", ".R", ".rst", ".spec", ".rb", ".rs", ".sas", ".sass", ".scala", ".scm", ".ss", ".scss", ".sh", ".ksh", ".bash", ".siv", ".sieve", ".slim", ".st", ".tpl", ".sml", ".sig", ".fun", ".smackspec", ".soy", ".rq", ".sparql", ".sql", ".nut", ".styl", ".swift", ".text", ".ltx", ".tex", ".v", ".sv", ".svh", ".tcl", ".textile", ".toml", ".1", ".2", ".3", ".4", ".5", ".6", ".7", ".8", ".9", ".ttcn", ".ttcn3", ".ttcnpp", ".cfg", ".ttl", ".ts", ".tsx", ".webidl", ".vb", ".vbs", ".vtl", ".v", ".vhd", ".vhdl", ".vue", ".xml", ".xsl", ".xsd", ".svg", ".xy", ".xquery", ".ys", ".yaml", ".yml", ".z80", ".mscgen", ".mscin", ".msc", ".xu", ".msgenny", ".wat", ".wast" };
    private static readonly string[] imageViewerSupportTypes =
    {
        ".jpg",".jpeg",".png",".gif",".bmp",".svg",".webp",".jfif",".ico"
    };

    private static readonly Regex[] regex = {
        new Regex("extensions\\.conf",RegexOptions.CultureInvariant| RegexOptions.Compiled),
        new Regex("CMakeLists\\.txt",RegexOptions.CultureInvariant| RegexOptions.Compiled),
        new Regex("Dockerfile",RegexOptions.CultureInvariant| RegexOptions.Compiled),
        new Regex("(readme|contributing|history)\\.md",RegexOptions.CultureInvariant| RegexOptions.Compiled),
        new Regex("nginx.*\\.conf",RegexOptions.CultureInvariant| RegexOptions.Compiled),
        new Regex("PKGBUILD",RegexOptions.CultureInvariant| RegexOptions.Compiled),
        new Regex("Jenkinsfile",RegexOptions.CultureInvariant| RegexOptions.Compiled),
        new Regex("(BUCK|BUILD)",RegexOptions.CultureInvariant| RegexOptions.Compiled)
};
    public async Task<bool> OpenArtifact(FsArtifact artifact)
    {
        if (!CanOpen(artifact))
            return false;

        _currentArtifact = artifact;
        if (IsVideoSupported(_currentArtifact))
        {
            IsModalOpen = false;
            await NavigateToView(_currentArtifact);
        }
        else
        {
            IsModalOpen = true;
        }

        await InvokeAsync(StateHasChanged);
        return true;
    }

    private bool CanOpen(FsArtifact artifact)
    {
        if (IsVideoSupported(artifact))
            return true;
        if (IsSupported<ImageViewer>(artifact))
            return true;
        if (IsSupported<ZipViewer>(artifact))
            return true;
        if (IsSupported<TextViewer>(artifact))
            return true;

        return false;
    }

    public async Task NavigateToView(FsArtifact artifact)
    {
        await NativeNavigation.NavigateToVideoViewer(artifact.FullPath, OnBack);
    }

    private bool IsVideoSupported(FsArtifact? artifact)
    {
        if (artifact?.FileCategory == FileCategoryType.Video)
        {
            return true;
        }

        return false;
    }

    private bool IsSupported<TComponent>(FsArtifact? artifact)
        where TComponent : IFileViewerComponent
    {
        if (artifact is null)
            return false;

        if (IsVideoSupported(artifact))
            return true;
        if (typeof(TComponent) == typeof(ImageViewer) && imageViewerSupportTypes.Contains(artifact.FileExtension))
            return true;
        if (typeof(TComponent) == typeof(ZipViewer) && artifact.FileCategory == FileCategoryType.Zip)
            return true;
        if (typeof(TComponent) == typeof(TextViewer) && (textViewerSupportTypes.Contains(artifact.FileExtension) || regex.Any(r => r.IsMatch(artifact.Name))))
            return true;

        return false;
    }

    public async Task HandleBackAsync()
    {
        IsModalOpen = false;
        await OnBack.InvokeAsync();
    }

    public async Task HandleNavigateAsync(string path)
    {
        IsModalOpen = false;
        await NavigationFolderCallback.InvokeAsync(path);
    }
}