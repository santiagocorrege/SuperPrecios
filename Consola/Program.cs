using HtmlAgilityPack;

namespace Consola
{
    internal class Program
    {

        static async Task Main()
        {
            string url = "https://shop.tata.com.uy/hogar-muebles-y-jardin/cuidado-del-hogar-y-lavanderia/productos-de-limpieza";

            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);

                // Buscar los contenedores de productos
                var productos = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'product-item')]")
                                 ?.Take(10) // Tomar los primeros 10 productos
                                 .ToList();

                if (productos == null || productos.Count == 0)
                {
                    Console.WriteLine("No se encontraron productos.");
                    return;
                }

                Console.WriteLine("Lista de productos:");

                foreach (var producto in productos)
                {
                    // Obtener el nombre del producto
                    var nombreNodo = producto.SelectSingleNode(".//h2[contains(@class, 'product-title')]");
                    string nombre = nombreNodo != null ? nombreNodo.InnerText.Trim() : "Nombre no disponible";

                    // Obtener el precio
                    var precioNodo = producto.SelectSingleNode(".//span[contains(@class, 'price')]");
                    string precio = precioNodo != null ? precioNodo.InnerText.Trim() : "Precio no disponible";

                    Console.WriteLine($"Producto: {nombre} - Precio: {precio}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar el scraping: {ex.Message}");
            }
        }
    }
}
