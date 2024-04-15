using Modthara.App.ViewModels;

namespace Modthara.App.Routing;

public class Router(Func<Type, ViewModelBase> createViewModel)
{
    private ViewModelBase? _currentViewModel;

    protected ViewModelBase CurrentViewModel
    {
        set
        {
            if (value == _currentViewModel)
            {
                return;
            }

            _currentViewModel = value;
            OnCurrentViewModelChanged(value);
        }
    }

    protected Func<Type, ViewModelBase> CreateViewModel { get; } = createViewModel;

    public event Action<ViewModelBase>? CurrentViewModelChanged;

    public virtual ViewModelBase GoTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewModel = InstantiateViewModel<TViewModel>();
        CurrentViewModel = viewModel;

        return viewModel;
    }

    protected TViewModel InstantiateViewModel<TViewModel>() where TViewModel : ViewModelBase
    {
        return (TViewModel)Convert.ChangeType(CreateViewModel(typeof(TViewModel)), typeof(TViewModel));
    }

    private void OnCurrentViewModelChanged(ViewModelBase viewModel)
    {
        CurrentViewModelChanged?.Invoke(viewModel);
    }
}
