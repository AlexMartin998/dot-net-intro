using ProductApi.Models;
using System.Collections.Concurrent;

namespace ProductApi.Services;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
    Product Create(Product product);
    void Update(int id, Product product);
    void Delete(int id);
}

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<int, Product> _products = new();
    private int _nextId = 1;

    public IEnumerable<Product> GetAll() => _products.Values;

    public Product? GetById(int id) => _products.GetValueOrDefault(id);

    public Product Create(Product product)
    {
        product.Id = _nextId++;
        _products[product.Id] = product;
        return product;
    }

    public void Update(int id, Product product)
    {
        if (!_products.ContainsKey(id))
            throw new KeyNotFoundException();

        product.Id = id;
        _products[id] = product;
    }

    public void Delete(int id) => _products.TryRemove(id, out _);
}
