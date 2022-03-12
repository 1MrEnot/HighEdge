namespace Straonit.HighEdge.Ioc;

using Core.SplitSecret;
using Secret;

public static class ShamirAlgortihmsRegistration
{
    public static IServiceCollection AddShamirServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ISecretSplitter, ShamirSecretSplitter>();
        serviceCollection.AddSingleton<ISecretCombiner, ShamirSecretCombiner>();

        return serviceCollection;
    }
}