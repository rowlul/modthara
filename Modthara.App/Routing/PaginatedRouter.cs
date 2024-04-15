using CommunityToolkit.Mvvm.ComponentModel;

using Modthara.App.ViewModels;

namespace Modthara.App.Routing;

public class PaginatedRouter<TViewModelBase>(Func<Type, ViewModelBase> createViewModel)
    : Router<TViewModelBase>(createViewModel) where TViewModelBase : ObservableObject
{
    private int _currentIndex = -1;
    private List<TViewModelBase> _viewModels = [];

    public bool HasNext => _viewModels.Count > 0 && _currentIndex < _viewModels.Count - 1;
    public bool HasPrevious => _currentIndex > 0;

    public void Push(TViewModelBase viewModel)
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

    public TViewModelBase? Go(int offset)
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

    public TViewModelBase? Back() => HasPrevious ? Go(-1) : null;
    public TViewModelBase? Forward() => HasNext ? Go(1) : null;

    public override TViewModel GoTo<TViewModel>()
    {
        var viewModel = InstantiateViewModel<TViewModel>();
        CurrentViewModel = viewModel;
        Push(viewModel);

        return viewModel;
    }

    private const uint MaxPageIndex = 100;
}
