namespace BusinessSystem.Helpers
{
    public static class ProductHelper
    {
        // create a Models.Product from RemoteStorageService.Models.Product
        public static Models.Product CreateProductFromRemoteProduct(BusinessSystem.Services.RemoteStorageService.Models.Product remoteProduct)
        {
            // check type on remoteProduct and generate correct product type
            switch (remoteProduct.Type)
            {
                case Models.Constants.Constants.ProductTypes.Book:
                    return new Models.Book
                    {
                        Id = remoteProduct.Id,
                        Name = remoteProduct.Name,
                        Price = remoteProduct.Price,
                        Stock = remoteProduct.Stock,
                        Genre = !string.IsNullOrWhiteSpace(remoteProduct.Genre) ? remoteProduct.Genre : string.Empty,
                        Format = !string.IsNullOrWhiteSpace(remoteProduct.Format) ? remoteProduct.Format : string.Empty,
                        Language = !string.IsNullOrWhiteSpace(remoteProduct.Language) ? remoteProduct.Language : string.Empty,
                        Author = ""
                    };
                case Models.Constants.Constants.ProductTypes.Game:
                    return new Models.Game
                    {
                        Id = remoteProduct.Id,
                        Name = remoteProduct.Name,
                        Price = remoteProduct.Price,
                        Stock = remoteProduct.Stock,
                        Platform = !string.IsNullOrWhiteSpace(remoteProduct.Platform) ? remoteProduct.Platform : string.Empty
                    };
                case Models.Constants.Constants.ProductTypes.Movie:
                    return new Models.Movie
                    {
                        Id = remoteProduct.Id,
                        Name = remoteProduct.Name,
                        Price = remoteProduct.Price,
                        Stock = remoteProduct.Stock,
                        PlayTime = remoteProduct.Playtime.HasValue ? remoteProduct.Playtime.Value : 0,
                        Format = !string.IsNullOrWhiteSpace(remoteProduct.Format) ? remoteProduct.Format : string.Empty
                    };
                default:
                    return new Models.Product
                    {
                        Id = remoteProduct.Id,
                        Name = remoteProduct.Name,
                        Price = remoteProduct.Price,
                        Stock = remoteProduct.Stock
                    };
            }
        }
    }
}
