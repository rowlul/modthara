using Modthara.App.ViewModels;

namespace Modthara.App.Routing;

public class PaginatedRouter : Router
{
    private int _currentIndex = -1;
    private List<ViewModelBase> _viewModels = [];

    public bool HasNext => _viewModels.Count > 0 && _currentIndex < _viewModels.Count - 1;
    public bool HasPrevious => _currentIndex > 0;

    public PaginatedRouter(Func<Type, ViewModelBase> createViewModel) : base(createViewModel)
    {
    }

    public void Push(ViewModelBase viewModel)
    {
        if (HasNext)
        {
            _viewModels = _viewModels.Take(_currentIndex + 1).ToList();
        }

        _viewModels.Add(viewModel);
        _currentIndex = _viewModels.Count - 1;
        if (_viewModels.Count > MaxPageIndex)
        {
            _viewModels.RemoveAt(0);
        }
    }

    public ViewModelBase? Go(int offset)
    {
        if (offset == 0)
        {
            return null;
        }

        var index = _currentIndex + offset;
        if (index < 0 || index > _viewModels.Count - 1)
        {
            return null;
        }

        _currentIndex = index;

        var viewModel = _viewModels.ElementAt(index);
        CurrentViewModel = viewModel;

        return viewModel;
    }

    public ViewModelBase? Back() => HasPrevious ? Go(-1) : null;
    public ViewModelBase? Forward() => HasNext ? Go(1) : null;

    public override ViewModelBase GoTo<TViewModel>()
    {
        var viewModel = InstantiateViewModel<ViewModelBase>();
        CurrentViewModel = viewModel;
        Push(viewModel);

        return viewModel;
    }

    private const uint MaxPageIndex = 100;
}
