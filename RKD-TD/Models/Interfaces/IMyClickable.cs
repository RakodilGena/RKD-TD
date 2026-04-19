using System;

namespace RKD_TD.Models.Interfaces;

internal interface IMyClickable
{
    public event EventHandler? Clicked;
}