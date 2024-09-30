﻿using CommunityToolkit.Mvvm.ComponentModel;

using Modthara.UI.ViewModels;

namespace Modthara.UI.Routing;

public class Router<TViewModelBase> where TViewModelBase : ObservableObject
{
    private TViewModelBase? _currentViewModel;

    public Router(Func<Type, ViewModelBase> createViewModel)
    {
        CreateViewModel = createViewModel;
    }

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

    protected Func<Type, ViewModelBase> CreateViewModel { get; }

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
