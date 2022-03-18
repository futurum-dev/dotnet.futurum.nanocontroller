namespace Futurum.NanoController.Sample.Features;

public static class FeatureMapper
{
    public static FeatureDto Map(Feature domain) =>
        new(domain.Name);
}