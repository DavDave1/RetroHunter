using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaSetMaker.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaSetMaker.ViewModels
{
    public partial class TreeViewItemModel(MainViewModel mainVm) : ViewModelBase
    {
        public virtual string Title { get; } = "";

        public virtual bool IsSelectable { get; } = true;

        public virtual string IconSrc { get; } = "";

        public Bitmap? Icon => HasIcon ? ImageHelper.LoadFromResource(new Uri(IconSrc)) : null;

        public bool HasIcon => IconSrc != string.Empty;

        public string StatusColor => "";

        [ObservableProperty]
        private bool _isChecked = true;


        [ObservableProperty]
        private List<TreeViewItemModel> _children = [];

        partial void OnIsCheckedChanged(bool value)
        {
            Children.ForEach(c => c.IsChecked = value);
            OnItemChecked(value);
        }

        protected virtual void OnItemChecked(bool value)
        {
        }

        protected virtual bool CanShowDetails => false;
        protected virtual bool CanCompress => false;

        [RelayCommand]
        private async Task ShowDetails()
        {
            await mainVm.ShowDetails(this);
        }

        [RelayCommand]
        private async Task Compress()
        {
            await mainVm.Compress(this);
        }


    }
}
