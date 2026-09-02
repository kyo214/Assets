namespace Fusion;

public delegate void ChangedDelegate<T>(Changed<T> changed) where T : NetworkBehaviour;
