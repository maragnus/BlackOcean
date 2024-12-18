namespace BlackOcean.Simulation.ControlPanels;

[PublicAPI]
public readonly record struct Band(double Value, Status Status)
{
    public readonly static Band Default = new(0.0, Status.None);

    public static Band[] Build(Status status, params (double, Status)[] args)
    {
        var bands = new Band[1 + args.Length];
        bands[0] = new Band(0, status);
        for (var i = 0; i < args.Length; i++)
            bands[i + 1] = new Band(args[i].Item1, args[i].Item2);
        return bands;
    }
}