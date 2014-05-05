﻿namespace ScriptKit.CLR.Html
{
    /// <summary>
    /// This value specifies whether the user agent is allowed to synthesize bold or oblique font faces when a font family lacks bold or italic faces.
    /// </summary>
    [ScriptKit.CLR.Ignore]
    [ScriptKit.CLR.EnumEmit(EnumEmit.StringNameLowerCase)]
    [ScriptKit.CLR.Name("String")]
    public enum FontSynthesis
    {
        /// <summary>
        /// user agent is not allowed to synthesize bold or oblique font when not available.
        /// </summary>
        None,

        /// <summary>
        /// user agent is only allowed to synthesize bold font when not available but not oblique.
        /// </summary>
        Weight, 
        
        /// <summary>
        /// user agent is only allowed to synthesize oblique font when not available but not bold.
        /// </summary>
        Style,

        /// <summary>
        /// user agent is allowed to synthesize both bold and oblique.
        /// </summary>
        [ScriptKit.CLR.Name("weight style")]
        WeightStyle
    }
}
