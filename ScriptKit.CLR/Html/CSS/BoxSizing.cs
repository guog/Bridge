﻿namespace ScriptKit.CLR.Html
{
    /// <summary>
    /// The box-sizing CSS property is used to alter the default CSS box model used to calculate widths and heights of elements.
    /// </summary>
    [ScriptKit.CLR.Ignore]
    [ScriptKit.CLR.EnumEmit(EnumEmit.StringNameLowerCase)]
[ScriptKit.CLR.Name("String")]
    public enum BoxSizing
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// The width and height properties include the padding and border, but not the margin. This is the box model used by Internet Explorer when the document is in Quirks mode.
        /// </summary>
        [ScriptKit.CLR.Name("border-box")]
        BorderBox,

        /// <summary>
        /// The width and height properties include the padding size, and do not include the border or margin.
        /// </summary>
        [ScriptKit.CLR.Name("padding-box")]
        PaddingBox,

        /// <summary>
        /// This is the default style as specified by the CSS standard. The width and height properties are measured including only the content, but not the border, margin, or padding.
        /// </summary>
        [ScriptKit.CLR.Name("content-box")]
        ContentBox,
        
        /// <summary>
        /// 
        /// </summary>
        Inherit
    }
}
