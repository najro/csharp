using BusinessSystem.Models;
using BusinessSystem.Models.Constants;

namespace BusinessSystem.Extensions
{
    public static class ProductExtension
    {
        public static string GetTypeNameTranslation(this Product typeName)
        {
            if (typeName == null)
            {
                return string.Empty;
            }
            else if (typeName is Game)
            {
                return Constants.ProuctTypesTranslaton.Game;
            }
            else if (typeName is Book)
            {
                return Constants.ProuctTypesTranslaton.Book;
            }
            else if (typeName is Movie)
            {
                return Constants.ProuctTypesTranslaton.Movie;
            }else
            {
                return Constants.ProuctTypesTranslaton.Produkt;
            }
        }
    }
}
