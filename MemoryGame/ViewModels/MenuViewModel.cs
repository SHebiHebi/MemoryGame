using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MemoryGame.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        private DelegateCommand<RoutedEventArgs> startCommand;
        public DelegateCommand<RoutedEventArgs> StartCommand =>
            startCommand ?? (startCommand = new DelegateCommand<RoutedEventArgs>(StartExecute));

        public MenuViewModel(IRegionManager regionMgr)
        {
            regionManager = regionMgr;
        }

        private void StartExecute(RoutedEventArgs e)
        {
            regionManager.RequestNavigate("ContentRegion", "Main");
        }
    }
}
