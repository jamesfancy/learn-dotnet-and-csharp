using FancyIdea.DependencyInjection.Abstraction;

namespace FancyIdea.DependencyInjection.Sample;

internal class BusinessService {
    protected IRepository Repository { get; }

    public BusinessService(IRepository repository) {
        Repository = repository;
    }

    public void ProcessBusiness() {
        WriteLine($"[{Repository.GetDbType()}] {Repository.GetVersion()}");
    }
}
