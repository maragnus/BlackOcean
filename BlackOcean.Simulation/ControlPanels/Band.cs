namespace BlackOcean.Simulation.ControlPanels;

public readonly record struct Band(double Value, Status Status)
{
    public readonly static Band Default = new(0.0, Status.Safe);

    public static Band[] Build(Status status, params (double, Status)[] args)
    {
        var bands = new Band[1 + args.Length / 2];
        bands[0] = new Band(0, status);
        for (int i = 0; i < args.Length; i++)
            bands[i] = new Band(args[i].Item1, args[i].Item2);
        return bands;
    }
}