using System;

/// <summary>
/// Handling waypoint reaching and direction changing of player.
/// </summary>
/// <param name="sender"></param>
/// <param name="isWalkingForward"></param>
public delegate void CharacterMovementEventHandler(Object sender, bool isWalkingForward);