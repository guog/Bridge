﻿namespace ScriptKit.CLR.Html
{
    /// <summary>
    /// The font-style CSS property allows italic or oblique faces to be selected within a font-family.
    /// </summary>
    [ScriptKit.CLR.Ignore]
    [ScriptKit.CLR.EnumEmit(EnumEmit.StringNameLowerCase)]
    [ScriptKit.CLR.Name("String")]
    public enum FontStyle
    {
        /// <summary>
        /// 
        /// </summary>
        Inherit,

        /// <summary>
        /// Selects a font that is classified as normal within a font-family
        /// </summary>
        Normal, 
        
        /// <summary>
        /// Selects a font that is labeled italic, if that is not available, one labeled oblique
        /// </summary>
        Italic,

        /// <summary>
        /// Selects a font that is labeled oblique
        /// </summary>
        Oblique
    }
}
