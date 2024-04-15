using CommunityToolkit.Mvvm.ComponentModel;

using Modthara.App.ViewModels;

namespace Modthara.App.Routing;

public class Router<TViewModelBase>(Func<Type, ViewModelBase> createViewModel) where TViewModelBase : ObservableObject
{
    private TViewModelBase? _currentViewModel;

    protected TViewModelBase CurrentViewModel
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

    public event Action<TViewModelBase>? CurrentViewModelChanged;

    public virtual TViewModel GoTo<TViewModel>() where TViewModel : TViewModelBase
    {
        var viewModel = InstantiateViewModel<TViewModel>();
        CurrentViewModel = viewModel;

        return viewModel;
    }

    protected TViewModel InstantiateViewModel<TViewModel>() where TViewModel : TViewModelBase
    {
        return (TViewModel)Convert.ChangeType(CreateViewModel(typeof(TViewModel)), typeof(TViewModel));
    }

    private void OnCurrentViewModelChanged(TViewModelBase viewModel)
    {
        CurrentViewModelChanged?.Invoke(viewModel);
    }
}
