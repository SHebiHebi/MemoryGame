using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MemoryGame.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        public DelegateCommand<string> NavigateCommand { get; private set; }

        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(IRegionManager regionMgr)
        {
            regionManager = regionMgr;
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(Views.Menu));

            // NavigateCommand = new DelegateCommand<string>(Navigate);
            regionManager.RequestNavigate("ContentRegion", "Menu");
        }

        //private void Navigate(string navigatePath)
        //{
        //    if (navigatePath != null)
        //        regionManager.RequestNavigate("ContentRegion", navigatePath);
        //}
    }
}
