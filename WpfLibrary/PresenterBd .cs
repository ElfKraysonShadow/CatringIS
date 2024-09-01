using System;
using System.Windows;
using WpfLibrary.PresenterBD;
namespace WpfLibrary
{
    public class PresenterBd
    {

        private static Window _view;
        private string bd;

        public PresenterBd(Window view, string BD)
        {
            bd = BD;
            _view = view;
        }
        public IPresenterCommon ShowBank()
        {
            var presenterBank = new PresenterBankBd(_view, bd);
            return presenterBank;
        }
        public IPresenterCommon ShowFoodGroup()
        {
            var presenterFoodGroup = new PresenterFoodGroupBd(_view, bd);
            return presenterFoodGroup;
        }
        public IPresenterCommon ShowAddress()
        {
            var presenterAddress = new PresenterAddressBd(_view, bd);
            return presenterAddress;
        }
        public IPresenterCommon ShowApplication()
        {
            var presenterApplication = new PresenterApplicationIngBd(_view, bd);
            return presenterApplication;
        }
        public IPresenterCommon ShowProvider()
        {
            var presenterProvider = new PresenterProviderBd(_view, bd);
            return presenterProvider;
        }
        public IPresenterCommon ShowIngredient()
        {
            var presenterIngredient = new PresenterIngredientBd(_view, bd);
            return presenterIngredient;
        }
        public IPresenterCommon ShowSupply()
        {
            var presenterSupply = new PresenterSupplyIngredientBd(_view, bd);
            return presenterSupply;
        }
        public IPresenterCommon ShowFood()
        {
            var presenterFood = new PresenterFoodBd(_view, bd);
            return presenterFood;
        }
        public IPresenterCommon ShowSoldVolume()
        {
            var presenterSoldVolume = new PresenterSoldVolumeBd(_view, bd);
            return presenterSoldVolume;
        }
        public IPresenterCommon ShowRecipe()
        {
            var presenterRecipe = new PresenterRecipeIngredientBd(_view, bd);
            return presenterRecipe;
        }
    }
}
