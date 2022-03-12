namespace Straonit.HighEdge.Ioc;

using Core.Secret;
using Secret;

public static class ShamirAlgortihmsRegistration
{
    public static IServiceCollection AddShamirServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ISecretSplitter, ShamirSecretSplitter>();
        serviceCollection.AddSingleton<ISecretMerger, ShamirSecretCombiner>();

        return serviceCollection;
    }
}