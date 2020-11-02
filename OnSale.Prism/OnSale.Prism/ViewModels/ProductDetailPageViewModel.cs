using OnSale.Common.Entities;
using OnSale.Common.Responses;
using OnSale.Prism.Helpers;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace OnSale.Prism.ViewModels
{
    public class ProductDetailPageViewModel : ViewModelBase
    {
        private CowResponse _product;
        private ObservableCollection<CowImage> _images;


        public ProductDetailPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Languages.Product;
        }


        public ObservableCollection<CowImage> Images
        {
            get => _images;
            set => SetProperty(ref _images, value);
        }

        public CowResponse Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("product"))
            {
                Product = parameters.GetValue<CowResponse>("product");
                Images = new ObservableCollection<CowImage>(Product.ProductImages);
            }
        }


    }
}
