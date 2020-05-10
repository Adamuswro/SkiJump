public enum hillType {
    ///<summary>
    /// Rozmiar skoczni
    /// </summary>
    small,
medium,
large,
mamut,
}

public enum Skill
///<summary>
/// Respresent skill factor, could be use with style skill or distance skill
/// </summary>
{
    veryWeak,
    weak,
    average,
    good,
    veryGood
}

public enum playerState
///<summary>Gdzie znajduje się skoczek: belka, najazd, w trakcie lądowania, itp.</summary>
{
    Wait,           ///<summary>Skoczek na belce</summary>
    Ride,           ///<summary>Skoczek podczas najazdu</summary>
    Fly,             ///<summary>Skoczek podczas lotu</summary>
    DuringTwoLegs,      ///<summary>Skoczek w trakcie lądowania na dwie nogi</summary>
    Landing,            //Jest w trakcie lądowania
    LandingTwoLegs,     ///<summary>Skoczek wylądował na dwie nogi</summary>
    DuringTelemark,     ///<summary>Skoczek w trakcie telemarku</summary>
    Telemark,           ///<summary>Skoczek wylądował telemarkiem</summary>
    Fall,               ///<summary>Skoczek przewrócił się</summary>
    Finish,             ///<summary>Skoczek po wylądowanym skoku</summary>
}

public enum jumperBody  
///<summary>
/// Body of the jumper
/// </summary>
{
    Head,
    Body,
    UpperArm,
    LowerArm,
    Hand,
    UpperLeg,
    LowerLeg,
    Foot,
    Ski,
}

public enum hills
///<summary>
/// Body of the jumper
/// </summary>
{
    Prototype,
}

public enum CompetitionStatus
{
    round,
    scoreTable,
}