namespace FancyIdea.DependencyInjection.Abstraction;

// 将业务接口抽象出来，以便入口程序（这里指 DependencyInjectionSample），基于抽象层来调用业务
public interface IRepository {
    string GetDbType();
    string GetVersion();
}
