﻿namespace Framework.Doc
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Component serialization, deserialization mapping.
    /// </summary>
    internal enum DataEnum
    {
        None,

        AppDoc,

        MdDoc,

        MdPage,

        MdSpace,

        MdParagraph,

        MdNewLine,

        MdComment,

        MdTitle,

        MdBullet,

        MdCode,

        MdImage,

        MdBracket,

        MdQuotation,

        MdSymbol,

        MdSpecial,

        MdFont,

        MdLink,

        MdContent,

        SyntaxOmit,
        
        SyntaxDoc,

        SyntaxPage,

        SyntaxComment,

        SyntaxTitle,

        SyntaxBullet,

        SyntaxCode,

        SyntaxFont,

        SyntaxLink,

        SyntaxImage,

        SyntaxCustomNote,

        SyntaxPageBreak,

        SyntaxParagraph,

        SyntaxNewLine,

        SyntaxContent,

        SyntaxIgnore,

        HtmlDoc,

        HtmlPage,

        HtmlComment,

        HtmlTitle,
        
        HtmlParagraph,

        HtmlBulletList,

        HtmlBulletItem,

        HtmlFont,

        HtmlLink,

        HtmlImage,
        
        HtmlCode,

        HtmlCustomNote,

        HtmlContent,
    }

    /// <summary>
    /// Component data.
    /// </summary>
    internal sealed class DataDoc
    {
        public Registry Registry;

        public int Id { get; set; }

        public DataEnum DataEnum { get; set; }

        /// <summary>
        /// Gets Owner. Not serialized.
        /// </summary>
        public DataDoc Owner;

        /// <summary>
        /// Gets Index. This is the index of this object in Owner.List. Not serialized.
        /// </summary>
        public int Index;

        /// <summary>
        /// Gets List. Can be null if no children. See also method ListGet();
        /// </summary>
        public List<DataDoc> List { get; set; }

        public List<DataDoc> ListGet()
        {
            if (List == null)
            {
                return new List<DataDoc>();
            }
            else
            {
                return List;
            }
        }

        public int ListCount()
        {
            return List != null ? List.Count : 0;
        }

        public DataDoc Last(bool isOrDefault = false, int offset = 0)
        {
            DataDoc result = null;
            int index = ListCount() - 1 - offset;
            bool isOutOfRange = index < 0 || index > List.Count;
            if (isOrDefault == false)
            {
                result = List[index];
            }
            else
            {
                if (!isOutOfRange)
                {
                    result = List[index];
                }
            }

            return result;
        }

        private Component componentCache;

        public Component Component()
        {
            var result = componentCache;
            if (result == null)
            {
                result = Registry.Deserialize(this);
                componentCache = result;
            }
            return result;
        }

        public string Text { get; set; }

        public bool IsOmit { get; set; }

        public int IndexBegin { get; set; }

        public int IndexEnd { get; set; }

        public bool IsCommentEnd { get; set; }

        public int TitleLevel { get; set; }

        public MdBracketEnum BracketEnum { get; set; }

        public bool IsBracketEnd { get; set; }

        public MdQuotationEnum QuotationEnum { get; set; }
        
        public MdSymbolEnum SymbolEnum { get; set; }

        public MdFontEnum FontEnum { get; set; }

        public int TokenIdBegin { get; set; }

        public int TokenIdEnd { get; set; }

        public int? MdDocId { get; set; }

        public int? SyntaxDocOneId { get; set; }

        public int? SyntaxDocTwoId { get; set; }

        public int? SyntaxDocThreeId { get; set; }

        public int? SyntaxDocFourId { get; set; }

        public int? HtmlDocId { get; set; }

        public int? SyntaxId { get; set; }

        public string Link { get; set; }

        public string LinkText { get; set; }

        public string CodeLanguage { get; set; }

        public string CodeText { get; set; }

        public bool IsCreateNew { get; set; }
    }

    /// <summary>
    /// Parse steps.
    /// </summary>
    public enum ParseEnum
    {
        None = 0,

        ParseOne = 1,

        ParseTwo = 2,

        ParseThree = 3,
        
        ParseFour = 4,
        
        ParseHtml = 5,
    }

    /// <summary>
    /// Component registry.
    /// </summary>
    internal class Registry
    {
        public Registry()
        {
            // Doc
            Add(typeof(AppDoc));

            // Token
            Add(typeof(MdDoc));
            Add(typeof(MdPage));
            Add(typeof(MdSpace));
            Add(typeof(MdParagraph));
            Add(typeof(MdNewLine));
            Add(typeof(MdComment));
            Add(typeof(MdTitle));
            Add(typeof(MdBullet));
            Add(typeof(MdCode));
            Add(typeof(MdImage));
            Add(typeof(MdBracket));
            Add(typeof(MdQuotation));
            Add(typeof(MdSymbol));
            Add(typeof(MdFont));
            Add(typeof(MdLink));
            Add(typeof(MdContent));

            // Syntax
            Add(typeof(SyntaxOmit)); // Needs to be first Syntax
            Add(typeof(SyntaxDoc));
            Add(typeof(SyntaxPage));
            Add(typeof(SyntaxComment));
            Add(typeof(SyntaxTitle));
            Add(typeof(SyntaxCode));
            Add(typeof(SyntaxBullet));
            Add(typeof(SyntaxFont));
            Add(typeof(SyntaxLink));
            Add(typeof(SyntaxImage));
            Add(typeof(SyntaxCustomNote));
            Add(typeof(SyntaxPageBreak));
            Add(typeof(SyntaxParagraph));
            Add(typeof(SyntaxNewLine));
            Add(typeof(SyntaxContent));
            Add(typeof(SyntaxIgnore)); // Needs to be last Syntax

            // Html
            Add(typeof(HtmlDoc));
            Add(typeof(HtmlPage));
            Add(typeof(HtmlComment));
            Add(typeof(HtmlTitle));
            Add(typeof(HtmlParagraph));
            Add(typeof(HtmlBulletList));
            Add(typeof(HtmlBulletItem));
            Add(typeof(HtmlFont));
            Add(typeof(HtmlLink));
            Add(typeof(HtmlImage));
            Add(typeof(HtmlCode));
            Add(typeof(HtmlCustomNote));
            Add(typeof(HtmlContent));
        }

        private void Add(Type type)
        {
            if (!Enum.TryParse<DataEnum>(type.Name, out var dataEnum))
            {
                throw new Exception(string.Format("Type not registered in enum! ({0}, {1})", nameof(DataEnum), type.Name));
            }
            List.Add(type);
            TypeList.Add(type, dataEnum);
            DataEnumList.Add(dataEnum, type);
        }

        /// <summary>
        /// (Type) Keeps sequence.
        /// </summary>
        public List<Type> List = new List<Type>();

        /// <summary>
        /// (Type, DataEnum)
        /// </summary>
        public Dictionary<Type, DataEnum> TypeList = new Dictionary<Type, DataEnum>();

        /// <summary>
        /// (DataEnum, Type)
        /// </summary>
        public Dictionary<DataEnum, Type> DataEnumList = new Dictionary<DataEnum, Type>();

        public int IdCount;

        /// <summary>
        /// (Id, Data)
        /// </summary>
        public Dictionary<int, DataDoc> IdList = new Dictionary<int, DataDoc>();

        /// <summary>
        /// Gets SyntaxRegistry. Available if one SyntaxRegistry has been created out of this Registry.
        /// </summary>
        public SyntaxRegistry SyntaxRegistry;

        /// <summary>
        /// Use in getter for component reference.
        /// </summary>
        public T ReferenceGet<T>(int? id) where T : Component
        {
            T result = null;
            if (id != null)
            {
                result = (T)IdList[id.Value].Component();
            }
            return result;
        }

        /// <summary>
        /// Use in setter for component reference.
        /// </summary>
        public static int? ReferenceSet(Component value)
        {
            return value?.Data.Id;
        }

        /// <summary>
        /// Gets or sets ParseEnum. This is the current parse step.
        /// </summary>
        public ParseEnum ParseEnum { get; set; }

        public Component Deserialize(DataDoc data)
        {
            var type = DataEnumList[data.DataEnum];
            var result = (Component)FormatterServices.GetUninitializedObject(type);
            result.Data = data;
            return result;
        }
    }

    /// <summary>
    /// Register new Component in Registry constructor.
    /// </summary>
    internal class Component
    {
        internal DataDoc Data;

        private Component(Component owner, Registry registry)
        {
            if (owner == null)
            {
                if (registry == null)
                {
                    registry = new Registry();
                }
                var dataEnum = registry.TypeList[GetType()];
                Data = new DataDoc { Registry = registry, Id = registry.IdCount += 1, DataEnum = dataEnum };
                registry.IdList.Add(Data.Id, Data);
            }
            else
            {
                UtilDoc.Assert(registry == null);
                registry = owner.Data.Registry;
                var dataEnum = registry.TypeList[GetType()]; // See also constructor Registry and enum DataEnum.
                Data = new DataDoc { Registry = registry, Id = registry.IdCount += 1, DataEnum = dataEnum, Owner = owner.Data };
                registry.IdList.Add(Data.Id, Data);
                if (owner.Data.List == null)
                {
                    owner.Data.List = new List<DataDoc>();
                }
                owner.Data.List.Add(Data);
                Data.Index = owner.Data.List.Count - 1;
            }
        }

        /// <summary>
        /// Constructor root.
        /// </summary>
        public Component() 
            : this(null, null)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Component(Component owner)
            : this(owner, null)
        {

        }

        /// <summary>
        /// Constructor registry, factory mode. Root with existing registry.
        /// </summary>
        public Component(Registry registry) 
            : this(null, registry)
        {

        }

        /// <summary>
        /// Gets Id. Unique Id in component tree.
        /// </summary>
        public int Id => Data.Id;

        /// <summary>
        /// Gets Owner. This is the components owner in tree.
        /// </summary>
        public Component Owner => Data.Owner?.Component();

        public T OwnerFind<T>() where T : Component
        {
            T result = null;
            var dataEnum = Data.Registry.TypeList[typeof(T)];
            var next = Data;
            do
            {
                if (next.DataEnum == dataEnum)
                {
                    result = (T)next.Component();
                    break;
                }
                next = next.Owner;
            } while (next != null);
            return result;
        }

        public IReadOnlyList<Component> List
        {
            get
            {
                var result = new List<Component>();
                if (Data.List != null)
                {
                    foreach (var item in Data.List)
                    {
                        result.Add(item.Component());
                    }
                }
                return result;
            }
        }

        private static void ListAll(Component component, List<Component> result)
        {
            result.Add(component);
            foreach (var item in component.List)
            {
                ListAll(item, result);
            }
        }

        /// <summary>
        /// Returns list of all child components recursive including this.
        /// </summary>
        public IReadOnlyList<Component> ListAll()
        {
            var result = new List<Component>();
            ListAll(this, result);
            return result;
        }

        /// <summary>
        /// Remove component from owner. Needs to be last component.
        /// </summary>
        public void Remove()
        {
            UtilDoc.Assert(Data.Owner != null);
            UtilDoc.Assert(Data.Owner.List.Last() == Data);
            Data.Owner.List.Remove(this.Data);
            Data.Owner = null;
        }

        /// <summary>
        /// Returns next or previous component.
        /// </summary>
        /// <param name="componentBeginEnd">Can be null for no range check.</param>
        /// <param name="offset">For example 1 (next) or -1 (previous)</param>
        private Component Next(Component componentBeginEnd, int offset)
        {
            Component result = null;
            if (Data.Owner != null) // Not root
            {
                UtilDoc.Assert(Data.Owner.List[Data.Index] == Data); // Index check
                if (this != componentBeginEnd) // Reached not yet begin or end
                {
                    if (Data.Index + offset >= 0 && Data.Index + offset < Data.Owner.List.Count) // There is a next component
                    {
                        result = Data.Owner.List[Data.Index + offset].Component(); // Move next
                    }
                }
            }
            return result;
        }

        public T Next<T>(T componentEnd) where T : Component
        {
            var result = Next(componentEnd, offset: 1);
            return (T)result;
        }

        /// <summary>
        /// Returns true, if next component.
        /// </summary>
        public static bool Next<T>(ref T component, T componentEnd) where T : Component
        {
            var result = component?.Next(componentEnd);
            if (result != null)
            {
                component = result;
            }
            return result != null;
        }

        /// <summary>
        /// Returns previous component.
        /// </summary>
        /// <param name="componentBegin">Can be null for no range check.</param>
        public T Previous<T>(T componentBegin) where T : Component
        {
            var result = Next(componentBegin, offset: -1);
            return (T)result;
        }

        public void Serialize(out string json)
        {
            var option = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault }; // Do not serialize default values.
            json = JsonSerializer.Serialize(Data, option);
        }

        private static void Deserialize(Registry registry, DataDoc owner, int index, DataDoc data)
        {
            data.Registry = registry;
            data.Owner = owner;
            data.Index = index;
            registry.IdList.Add(data.Id, data);
            if (data.List != null)
            {
                for (int i = 0; i < data.List.Count; i++)
                {
                    Deserialize(registry, data, i, data.List[i]);
                }
            }
        }

        public static T Deserialize<T>(string json) where T : Component
        {
            var data = JsonSerializer.Deserialize<DataDoc>(json);
            var registry = new Registry();
            Deserialize(registry, null, -1, data);
            var result = registry.Deserialize(data);
            return (T)result;
        }
    }

    /// <summary>
    /// Store and parse (*.md) pages.
    /// </summary>
    internal class AppDoc : Component
    {
        public AppDoc()
            : base()
        {
            MdDoc = new MdDoc(this);
            SyntaxDocOne = new SyntaxDoc(this);
            SyntaxDocTwo = new SyntaxDoc(this);
            SyntaxDocThree = new SyntaxDoc(this);
            SyntaxDocFour = new SyntaxDoc(this);
            HtmlDoc = new HtmlDoc(this);
        }

        /// <summary>
        /// Parse md pages.
        /// </summary>
        public void Parse()
        {
            // Init registries
            var mdRegistry = new MdRegistry(Data.Registry);
            var syntaxRegistry = new SyntaxRegistry(Data.Registry);

            // Lexer
            foreach (MdPage page in MdDoc.List)
            {
                // Clear token list
                if (page.Data.ListCount() > 0)
                {
                    page.Data.List = new List<DataDoc>();
                }

                // Lexer
                mdRegistry.Parse(page);
            }

            syntaxRegistry.Parse(this);
        }

        public MdDoc MdDoc
        {
            get => Data.Registry.ReferenceGet<MdDoc>(Data.MdDocId);
            set => Data.MdDocId = Registry.ReferenceSet(value);
        }

        public SyntaxDoc SyntaxDocOne
        {
            get => Data.Registry.ReferenceGet<SyntaxDoc>(Data.SyntaxDocOneId);
            set => Data.SyntaxDocOneId = Registry.ReferenceSet(value);
        }

        public SyntaxDoc SyntaxDocTwo
        {
            get => Data.Registry.ReferenceGet<SyntaxDoc>(Data.SyntaxDocTwoId);
            set => Data.SyntaxDocTwoId = Registry.ReferenceSet(value);
        }

        public SyntaxDoc SyntaxDocThree
        {
            get => Data.Registry.ReferenceGet<SyntaxDoc>(Data.SyntaxDocThreeId);
            set => Data.SyntaxDocThreeId = Registry.ReferenceSet(value);
        }

        public SyntaxDoc SyntaxDocFour
        {
            get => Data.Registry.ReferenceGet<SyntaxDoc>(Data.SyntaxDocFourId);
            set => Data.SyntaxDocFourId = Registry.ReferenceSet(value);
        }

        public HtmlDoc HtmlDoc
        {
            get => Data.Registry.ReferenceGet<HtmlDoc>(Data.HtmlDocId);
            set => Data.HtmlDocId = Registry.ReferenceSet(value);
        }
    }

    /// <summary>
    /// Span extensions.
    /// </summary>
    internal static class TextExtension
    {
        /// <summary>
        /// Returns char at index or null if out of range.
        /// </summary>
        public static char? Char(this ReadOnlySpan<char> text, int index)
        {
            char? result = null;
            if (index >= 0 && index < text.Length)
            {
                result = text.Slice(index, 1)[0];
            }
            return result;
        }

        public static bool StartsWith(this ReadOnlySpan<char> text, int index, string textFind)
        {
            return text.Slice(index).StartsWith(textFind, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Subset of Registry.
    /// </summary>
    internal class MdRegistry
    {
        public MdRegistry(Registry registry)
        {
            foreach (var type in registry.List)
            {
                if (type.IsSubclassOf(typeof(MdTokenBase)))
                {
                    var token = (MdTokenBase)Activator.CreateInstance(type);
                    List.Add(token);
                }
            }
        }

        /// <summary>
        /// (Token). Registry, factory mode.
        /// </summary>
        public List<MdTokenBase> List = new List<MdTokenBase>();

        public void Parse(MdPage owner)
        {
            var text = owner.Text.AsSpan();

            while (MdTokenBase.Parse(this, owner, text)) ;
        }
    }

    /// <summary>
    /// Md tree root. Containes (*.md) pages.
    /// </summary>
    internal class MdDoc : Component
    {
        public MdDoc(Component owner)
            : base(owner)
        {

        }
    }

    internal class MdPage : Component
    {
        public MdPage(MdDoc owner, string text)
            : base(owner)
        {
            Data.Text = text;
        }

        public string Text => Data.Text;
    }

    /// <summary>
    /// Base class for token.
    /// </summary>
    internal abstract class MdTokenBase : Component
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdTokenBase()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdTokenBase(MdPage owner, int length)
            : base(owner)
        {
            UtilDoc.Assert(length > 0);

            // IndexBegin is IndexEnd + 1 of previous
            var indexBegin = 0;
            if (Owner.List.Count > 1)
            {
                var previous = (MdTokenBase)Owner.Data.Last(offset: 1).Component();
                indexBegin = previous.IndexEnd + 1;
            }

            Data.IndexBegin = indexBegin;
            Data.IndexEnd = Data.IndexBegin + length - 1;

            UtilDoc.Assert(Data.IndexEnd <= owner.Text.Length);
        }

        public new MdPage Owner => (MdPage)base.Owner;

        public int IndexBegin => Data.IndexBegin;

        public int IndexEnd => Data.IndexEnd;

        public int Length => IndexEnd - IndexBegin + 1;

        internal void IndexEndSet(int index)
        {
            var isLast = Owner.Data.Last() == Data;
            UtilDoc.Assert(isLast, "Can only set IndexEnd of last token!");

            UtilDoc.Assert(index >= 0 && index < Owner.Text.Length, "Index out of range!");

            Data.IndexEnd = index;
        }

        /// <summary>
        /// Gets or sets IsOmit. See also class SyntaxOmit.
        /// </summary>
        public bool IsOmit
        {
            get => Data.IsOmit;
            set => Data.IsOmit = value;
        }

        /// <summary>
        /// Gets Text. This is the text between IndexBegin and IndexEnd.
        /// </summary>
        public string Text
        {
            get
            {
                return Owner.Text.Substring(Data.IndexBegin, Data.IndexEnd - Data.IndexBegin + 1);
            }
        }

        /// <summary>
        /// Returns current text parse index.
        /// </summary>
        public static int ParseIndex(MdPage owner)
        {
            var result = 0;

            MdTokenBase token = (MdTokenBase)owner.Data.Last(isOrDefault: true)?.Component();
            if (token != null)
            {
                result = token.IndexEnd + 1;
            }
            return result;
        }

        internal static bool Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, bool isExcludeContent = false)
        {
            var result = false;

            var index = ParseIndex(owner);

            foreach (var tokenParser in registry.List)
            {
                if (isExcludeContent)
                {
                    if (tokenParser.GetType() == typeof(MdContent))
                    {
                        break;
                    }
                }

                // Parse
                var countBefore = owner.Data.ListCount();
                tokenParser.Parse(registry, owner, text, index);
                var countAfter = owner.Data.ListCount();

                UtilDoc.Assert(countBefore <= countAfter);

                // A token has been created
                if (countBefore < countAfter)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        internal virtual void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {

        }
    }

    internal class MdParagraph : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdParagraph()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdParagraph(MdPage owner, int length)
            : base(owner, length)
        {

        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            int next = index;
            int count = 0;
            while (MdNewLine.Parse(text, next, out int length))
            {
                next += length;
                count += 1;
            }
            if (count >= 2)
            {
                new MdParagraph(owner, next - index);
            }
        }
    }

    internal class MdNewLine : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdNewLine()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdNewLine(MdPage owner, int length)
            : base(owner, length)
        {

        }

        /// <summary>
        /// Returns true, if NewLine.
        /// </summary>
        internal static bool Parse(ReadOnlySpan<char> text, int index, out int length)
        {
            var result = false;
            length = 0;

            var textFindList = new List<string>
            {
                "\r\n",
                "\r",
                "\n"
            };

            foreach (var textFind in textFindList)
            {
                if (text.StartsWith(index, textFind))
                {
                    result = true;
                    length = textFind.Length;
                    break;
                }
            }
            return result;
        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            if (Parse(text, index, out int length))
            {
                new MdNewLine(owner, length);
            }
        }
    }

    internal class MdSpace : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdSpace()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdSpace(MdPage owner, int length)
            : base(owner, length)
        {

        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            var length = 0;
            while (text.StartsWith(index + length, " "))
            {
                length += 1;
            }

            if (length > 0)
            {
                new MdSpace(owner, length);
            }
        }
    }

    internal class MdComment : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdComment()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdComment(MdPage owner, int length, bool isCommentEnd)
            : base(owner, length)
        {
            Data.IsCommentEnd = isCommentEnd;
        }

        public bool IsCommentEnd => Data.IsCommentEnd;

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            var commentBegin = "<!--";
            var commentEnd = "-->";
            if (text.StartsWith(index, commentBegin))
            {
                new MdComment(owner, commentBegin.Length, isCommentEnd: false);
            }
            if (text.StartsWith(index, commentEnd))
            {
                new MdComment(owner, commentEnd.Length, isCommentEnd: true);
            }
        }
    }

    internal class MdTitle : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdTitle()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdTitle(MdPage owner, int length, int titleLevel)
            : base(owner, length)
        {
            Data.TitleLevel = titleLevel;
        }

        public int TitleLevel => Data.TitleLevel;

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            if (!text.StartsWith(index, "####"))
            {
                if (text.StartsWith(index, "### "))
                {
                    new MdTitle(owner, length: 3, titleLevel: 3);
                }
                if (text.StartsWith(index, "## "))
                {
                    new MdTitle(owner, length: 2, titleLevel: 2);
                }
                if (text.StartsWith(index, "# "))
                {
                    new MdTitle(owner, length: 1, titleLevel: 1);
                }
            }
        }
    }

    internal class MdBullet : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdBullet()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdBullet(MdPage owner, int length)
            : base(owner, length)
        {

        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            if (text.StartsWith(index, "* "))
            {
                new MdBullet(owner, 1);
            }
        }
    }

    internal class MdImage : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdImage()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdImage(MdPage owner, int length)
            : base(owner, length)
        {

        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            if (text.StartsWith(index, "!["))
            {
                new MdImage(owner, 2);
            }
        }
    }

    internal enum MdBracketEnum
    {
        None,

        Round,

        Square,
    }

    internal class MdBracket : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdBracket()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdBracket(MdPage owner, int length, MdBracketEnum bracketEnum, bool isBracketEnd)
            : base(owner, length)
        {
            Data.BracketEnum = bracketEnum;
            Data.IsBracketEnd = isBracketEnd;
        }

        public MdBracketEnum BracketEnum => Data.BracketEnum;

        public bool IsBracketEnd => Data.IsBracketEnd;

        public string TextBracket
        {
            get
            {
                switch (BracketEnum)
                {
                    case MdBracketEnum.Round:
                        if (IsBracketEnd == false)
                        {
                            return "(";
                        }
                        else
                        {
                            return ")";
                        }
                    case MdBracketEnum.Square:
                        if (IsBracketEnd == false)
                        {
                            return "[";
                        }
                        else
                        {
                            return "]";
                        }
                    default:
                        throw new Exception("Enum unknown!");
                }
            }
        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            var textChar = text.Char(index);
            switch (textChar)
            {
                case '(':
                    new MdBracket(owner, 1, MdBracketEnum.Round, isBracketEnd: false);
                    break;
                case ')':
                    new MdBracket(owner, 1, MdBracketEnum.Round, isBracketEnd: true);
                    break;
                case '[':
                    new MdBracket(owner, 1, MdBracketEnum.Square, isBracketEnd: false);
                    break;
                case ']':
                    new MdBracket(owner, 1, MdBracketEnum.Square, isBracketEnd: true);
                    break;
            }
        }
    }

    internal enum MdQuotationEnum
    {
        None,

        Single,

        Double,
    }

    internal class MdQuotation : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdQuotation()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdQuotation(MdPage owner, int length, MdQuotationEnum quotationEnum)
            : base(owner, length)
        {
            Data.QuotationEnum = quotationEnum;
        }

        public MdQuotationEnum QuotationEnum => Data.QuotationEnum;

        public string TextQuotation
        {
            get
            {
                switch (QuotationEnum)
                {
                    case MdQuotationEnum.Single:
                        return "'";
                    case MdQuotationEnum.Double:
                        return "\"";
                    default:
                        throw new Exception("Enum unknown!");
                }
            }
        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            var textChar = text.Char(index);
            switch (textChar)
            {
                case '\'':
                    new MdQuotation(owner, 1, MdQuotationEnum.Single);
                    break;
                case '"':
                    new MdQuotation(owner, 1, MdQuotationEnum.Double);
                    break;
            }
        }
    }

    internal enum MdSymbolEnum
    {
        None,

        Equal,
    }

    internal class MdSymbol : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdSymbol()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdSymbol(MdPage owner, int length, MdSymbolEnum symbolEnum)
            : base(owner, length)
        {
            Data.SymbolEnum = symbolEnum;
        }

        public MdSymbolEnum SymbolEnum => Data.SymbolEnum;

        public string TextSymbol
        {
            get
            {
                switch (SymbolEnum)
                {
                    case MdSymbolEnum.Equal:
                        return "=";
                    default:
                        throw new Exception("Enum unknown!");
                }
            }
        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            var textChar = text.Char(index);
            switch (textChar)
            {
                case '=':
                    new MdSymbol(owner, 1, MdSymbolEnum.Equal);
                    break;
            }
        }
    }

    internal class MdCode : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdCode()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdCode(MdPage owner, int length)
            : base(owner, length)
        {

        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            if (text.StartsWith(index, "```") && text.Char(index + 3) != '`')
            {
                new MdCode(owner, 3);
            }
        }
    }

    internal enum MdFontEnum
    {
        None,

        Bold,

        Italic,
    }

    internal class MdFont : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdFont()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdFont(MdPage owner, int length, MdFontEnum fontEnum)
            : base(owner, length)
        {
            Data.FontEnum = fontEnum;
        }

        public MdFontEnum FontEnum => Data.FontEnum;

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            if (text.StartsWith(index, "**") && text.Char(index + 2) != '*')
            {
                new MdFont(owner, 2, MdFontEnum.Bold);
            }
            else
            {
                if (text.StartsWith(index, "*") && text.Char(index + 1) != '*')
                {
                    new MdFont(owner, 1, MdFontEnum.Italic);
                }
            }
        }
    }

    internal class MdLink : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdLink()
        {
            textList = new List<string>
            {
                "http://",
                "https://"
            };
        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdLink(MdPage owner, int length)
            : base(owner, length)
        {

        }

        private readonly List<string> textList;

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            foreach (var item in textList)
            {
                if (text.StartsWith(index, item))
                {
                    new MdLink(owner, item.Length);
                    break;
                }
            }
        }
    }

    internal class MdContent : MdTokenBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public MdContent()
            : base()
        {

        }

        /// <summary>
        /// Constructor instance token.
        /// </summary>
        public MdContent(MdPage owner, int length)
            : base(owner, length)
        {

        }

        internal override void Parse(MdRegistry registry, MdPage owner, ReadOnlySpan<char> text, int index)
        {
            MdContent token = null;
            while (ParseIndex(owner) < text.Length)
            {
                if (Parse(registry, owner, text, isExcludeContent: true) == false)
                {
                    if (token == null)
                    {
                        token = new MdContent(owner, 1);
                    }
                    else
                    {
                        token.IndexEndSet(token.IndexEnd + 1);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Subset of Registry.
    /// </summary>
    internal class SyntaxRegistry
    {
        public SyntaxRegistry(Registry registry)
        {
            UtilDoc.Assert(registry.SyntaxRegistry == null);
            registry.SyntaxRegistry = this;

            foreach (var type in registry.List)
            {
                if (type.IsSubclassOf(typeof(SyntaxBase)))
                {
                    // Create instance for registry, factory mode.
                    
                    // Call new SyntaxBase(registry);
                    var syntaxParser = (SyntaxBase)type.GetConstructor(new Type[] { typeof(Registry) }).Invoke(new object[] { registry }); // Activator
                    List.Add(syntaxParser);
                    TypeList.Add(type, syntaxParser);

                    // SchemaTypeList
                    var result = new SyntaxBase.RegistrySchemaResult();
                    syntaxParser.RegistrySchema(result);
                    SchemaOwnerTypeList.Add(type, result.List.Where(item => item.IsChild == true).Select(item => item.OwnerType).ToList());
                    foreach (var item in result.List.Select(item => item.OwnerType).ToList())
                    {
                        if (!SchemaTypeList.ContainsKey(item))
                        {
                            SchemaTypeList.Add(item, new List<Type>());
                        }
                        SchemaTypeList[item].Add(type);
                    }
                }
            }
        }

        /// <summary>
        /// (Syntax). Syntax parser list.
        /// </summary>
        public List<SyntaxBase> List = new List<SyntaxBase>();

        /// <summary>
        /// (Type, Syntax). Syntax parser list.
        /// </summary>
        public Dictionary<Type, SyntaxBase> TypeList = new Dictionary<Type, SyntaxBase>();

        /// <summary>
        /// (Type, Type). Owner type, child type. ParseTwo.
        /// </summary>
        public Dictionary<Type, List<Type>> SchemaTypeList = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// (Type, Type). Child type, owner type. ParseThree.
        /// </summary>
        public Dictionary<Type, List<Type>> SchemaOwnerTypeList = new Dictionary<Type, List<Type>>();

        public void Parse(AppDoc appDoc)
        {
            var mdDoc = appDoc.MdDoc;
            var syntaxDocOne = appDoc.SyntaxDocOne;
            var syntaxDocTwo = appDoc.SyntaxDocTwo;
            var syntaxDocThree = appDoc.SyntaxDocThree;
            var syntaxDocFour = appDoc.SyntaxDocFour;
            var htmlDoc = appDoc.HtmlDoc;

            // ParseOne
            appDoc.Data.Registry.ParseEnum = ParseEnum.ParseOne;
            foreach (MdPage page in mdDoc.List)
            {
                if (page.Data.ListCount() > 0)
                {
                    var tokenBegin = (MdTokenBase)page.Data.List[0].Component();
                    var tokenEnd = (MdTokenBase)page.Data.List[page.Data.List.Count - 1].Component();
                    var syntaxPage = new SyntaxPage(syntaxDocOne, tokenBegin, tokenEnd);
                    SyntaxBase.ParseOneMain(syntaxPage, this);
                }
            }

            // ParseTwo
            appDoc.Data.Registry.ParseEnum = ParseEnum.ParseTwo;
            SyntaxBase.ParseTwoMain(syntaxDocTwo, syntaxDocOne);

            // ParseThree
            appDoc.Data.Registry.ParseEnum = ParseEnum.ParseThree;
            SyntaxBase.ParseThreeMain(syntaxDocThree, syntaxDocTwo);

            // ParseFour
            appDoc.Data.Registry.ParseEnum = ParseEnum.ParseFour;
            SyntaxBase.ParseFourMain(syntaxDocFour, syntaxDocThree);

            // ParseHtml
            appDoc.Data.Registry.ParseEnum = ParseEnum.ParseHtml;
            syntaxDocFour.ParseHtml(htmlDoc);
        }
    }

    /// <summary>
    /// Base class for md syntax tree.
    /// </summary>
    internal abstract class SyntaxBase : Component
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxBase(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor for Doc.
        /// </summary>
        public SyntaxBase(Component owner)
            : base(owner)
        {
            Data.TokenIdBegin = -1;
            Data.TokenIdEnd = -1;
        }

        /// <summary>
        /// Create instance of this object in registry, factory mode.
        /// </summary>
        protected internal virtual SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            throw new Exception("Not implemented!");
        }

        /// <summary>
        /// Override this method to register possible owner types.
        /// </summary>
        internal virtual void RegistrySchema(RegistrySchemaResult result)
        {

        }

        internal class RegistrySchemaResult
        {
            public List<RegistrySchemaResultItem> List = new List<RegistrySchemaResultItem>();

            internal class RegistrySchemaResultItem
            {
                public Type OwnerType;

                public bool IsChild;
            }

            /// <summary>
            /// Register possible owner.
            /// </summary>
            /// <typeparam name="T">This syntax can be a child of owner.</typeparam>
            /// <param name="isChildDirect">This syntax can be a direct child of owner. Otherwise first owner in list is created in between.</param>
            public void AddOwner<T>(bool isChildDirect = true) where T : SyntaxBase
            {
                List.Add(new RegistrySchemaResultItem { OwnerType = typeof(T), IsChild = isChildDirect });
            }
        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxBase(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner)
        {
            UtilDoc.Assert(owner.Data.Registry.ParseEnum == ParseEnum.ParseOne);

            Data.TokenIdBegin = tokenBegin.Data.Id;
            Data.TokenIdEnd = tokenEnd.Data.Id;

            CreateValidate();
        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxBase(SyntaxBase owner, SyntaxBase syntax)
            : base(owner)
        {
            var parseEnum = owner.Data.Registry.ParseEnum;
            UtilDoc.Assert(parseEnum == ParseEnum.ParseTwo || parseEnum == ParseEnum.ParseThree || parseEnum == ParseEnum.ParseFour);

            Data.SyntaxId = syntax.Data.Id;
            Data.TokenIdBegin = syntax.TokenBegin.Data.Id;
            Data.TokenIdEnd = syntax.TokenEnd.Data.Id;

            // Enable, disable (for debug only) ParseThree validate.
            if (parseEnum == ParseEnum.ParseThree)
            {
                // Uncomment return to disable ParseThree validate.
                // No more exceptions but wrong sequence of components in output!
                
                // return;
            }

            if (owner is not SyntaxDoc)
            {
                var indexBegin = Index(syntax.Data.TokenIdBegin);
                var indexEnd = Index(syntax.Data.TokenIdEnd);
                var indexEndOwner = Index(owner.Data.TokenIdEnd);
                bool isShorten = indexEnd <= indexEndOwner;
                bool isExtend = indexBegin == indexEndOwner + 1;
                UtilDoc.Assert(isShorten ^ isExtend); // If exception in ParseThree, disable (for debug only) ParseThree validate above. Caused by wrong break in method ParseTwo calling method ParseTwoMainBreak.
                var next = owner.Data;
                while (next.DataEnum != DataEnum.SyntaxDoc)
                {
                    UtilDoc.Assert(next.Owner.List.Last() == next); // Modify TokenIdEnd only on last item
                    next.TokenIdEnd = syntax.Data.TokenIdEnd; // Modify TokenIdEnd
                    next = next.Owner;
                }
                UtilDoc.Assert(Index(owner.Data.TokenIdBegin) <= Index(owner.Data.TokenIdEnd));
            }

            CreateValidate();
        }

        /// <summary>
        /// Validate index ranges after create.
        /// </summary>
        private void CreateValidate()
        {
            var typeOwner = Data.Registry.DataEnumList[Data.Owner.DataEnum];
            if (!UtilDoc.IsSubclassOf(typeOwner, typeof(SyntaxDoc)))
            {
                UtilDoc.Assert(Index(Data.TokenIdBegin) <= Index(Data.TokenIdEnd));
                if (UtilDoc.IsSubclassOf(typeOwner, typeof(SyntaxBase)))
                {
                    UtilDoc.Assert(Index(Data.Owner.TokenIdBegin) <= Index(Data.TokenIdBegin));
                    UtilDoc.Assert(Index(Data.Owner.TokenIdEnd) >= Index(Data.TokenIdEnd));
                    if (Data.Index == 0)
                    {
                        UtilDoc.Assert(Index(Data.Owner.TokenIdBegin) == Index(Data.TokenIdBegin));
                    }
                    else
                    {
                        var indexPrevious = Index(Owner.Data.List[Data.Index - 1].TokenIdEnd);
                        var index = Index(Data.TokenIdBegin);

                        UtilDoc.Assert(indexPrevious + 1 == index);
                    }
                }
            }
        }

        private int Index(int id)
        {
            return Data.Registry.IdList[id].Index;
        }

        public MdTokenBase TokenBegin => (MdTokenBase)Data.Registry.IdList[Data.TokenIdBegin].Component();

        public MdTokenBase TokenEnd => (MdTokenBase)Data.Registry.IdList[Data.TokenIdEnd].Component();

        public void TokenEndSet(MdTokenBase tokenEnd)
        {
            var index = TokenEnd.Data.IndexEnd;
            var indexNew = tokenEnd.Data.IndexEnd;
            if (index < indexNew)
            {
                // Grow
                var indexOwnerMax = ((SyntaxBase)Owner).TokenEnd.Data.IndexEnd;
                UtilDoc.Assert(indexNew <= indexOwnerMax);
                Data.TokenIdEnd = tokenEnd.Data.Id;
                CreateValidate();
            }
            if (index > indexNew)
            {
                // Shrink
                throw new Exception("Not implemented!");
            }
        }

        /// <summary>
        /// Gets Text. This is the text between TokenBegin and TokenEnd.
        /// </summary>
        public string Text
        {
            get
            {
                var result = new StringBuilder();
                if (this is not SyntaxDoc)
                {
                    var tokenEnd = TokenEnd;
                    Component component = TokenBegin;
                    do
                    {
                        var token = (MdTokenBase)component;
                        result.Append(token.Text);
                    } while (Next(ref component, tokenEnd));
                }
                return UtilDoc.StringNull(result.ToString());
            }
        }

        /// <summary>
        /// Gets IsCreateNew. If true, syntax has been inserted by ParseThree.
        /// </summary>
        public bool IsCreateNew => Data.IsCreateNew;

        /// <summary>
        /// Returns token of currently parsed syntax.
        /// </summary>
        internal static MdTokenBase ParseOneToken(SyntaxBase syntax)
        {
            var result = syntax.TokenBegin;
            var last = syntax.Data.Last(isOrDefault: true);
            if (last != null)
            {
                var syntaxLast = (SyntaxBase)syntax.Data.Registry.IdList[last.Id].Component();
                result = syntaxLast.TokenEnd;
                result = result.Next(syntax.TokenEnd);
            }
            return result;
        }

        /// <summary>
        /// Returns true, if tokenBegin is a new line starting with token T.
        /// </summary>
        /// <typeparam name="T">Token to search.</typeparam>
        /// <param name="tokenSpace">Leading space.</param>
        /// <param name="token">Found token.</param>
        internal static bool ParseOneIsNewLine<T>(MdTokenBase tokenBegin, MdTokenBase tokenEnd, out MdSpace tokenSpace, out T token) where T : MdTokenBase
        {
            var result = false;

            bool isStart;
            tokenSpace = null;
            token = null;

            Component component = tokenBegin;

            // Leading start or NewLine or Paragraph
            var previous = tokenBegin.Previous((MdTokenBase)null);
            isStart = previous == null || previous is MdNewLine || previous is MdParagraph;
            // Leading Space
            if (component is MdSpace space)
            {
                tokenSpace = space;
                component = component.Next(tokenEnd);
            }
            // Token
            if (component is T)
            {
                token = (T)component;
            }

            if (isStart && token != null)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Returns true, if tokenBegin is Link.
        /// </summary>
        /// <param name="link">Detected Link.</param>
        internal static bool ParseOneIsLink(MdTokenBase tokenBegin, MdTokenBase tokenEnd, out MdTokenBase linkEnd, out string link)
        {
            bool result = false;
            linkEnd = tokenBegin;
            MdTokenBase next = tokenBegin;
            link = null;
            do
            {
                if (next is MdLink)
                {
                    if (next != tokenBegin)
                    {
                        break;
                    }
                }
                if (!(next is MdContent || next is MdLink))
                {
                    break;
                }
                linkEnd = next;
                link += next.Text;
                result = true;
            } while (Next(ref next, tokenEnd));
            return result;
        }

        /// <summary>
        /// Returns true, if tokenBegin contains LinkText.
        /// </summary>
        /// <param name="linkText">Detected LinkText.</param>
        internal static bool ParseOneIsLinkText(MdTokenBase tokenBegin, MdTokenBase tokenEnd, out MdTokenBase linkTextEnd, out string linkText)
        {
            var result = false;
            linkTextEnd = tokenBegin;
            MdTokenBase next = tokenBegin;
            linkText = null;
            do
            {
                if (!(next is MdContent || next is MdSpace || next is MdLink))
                {
                    break;
                }
                linkTextEnd = next;
                linkText += next.Text;
                result = true;
            } while (Next(ref next, tokenEnd));
            return result;
        }

        /// <summary>
        /// Main entry for ParseOne.
        /// </summary>
        internal static void ParseOneMain(SyntaxBase owner, SyntaxRegistry registry)
        {
            var tokenEnd = owner.TokenEnd;
            MdTokenBase tokenBegin;
            while ((tokenBegin = ParseOneToken(owner)) != null)
            {
                bool isFind = false;
                foreach (var syntaxParser in registry.List)
                {
                    var countBefore = owner.Data.ListCount();
                    syntaxParser.ParseOne(owner, tokenBegin, tokenEnd);
                    var countAfter = owner.Data.ListCount();

                    UtilDoc.Assert(countBefore <= countAfter);

                    if (countBefore < countAfter)
                    {
                        isFind = true;
                        break;
                    }
                }
                UtilDoc.Assert(isFind, "No syntax parser found!");
            }
        }

        /// <summary>
        /// Main entry for ParseOne.
        /// </summary>
        internal static void ParseOneMain(SyntaxBase owner, SyntaxBase syntax)
        {
            ParseOneMain(owner, syntax.Data.Registry.SyntaxRegistry);
        }

        internal static bool ParseTwoIsText(SyntaxBase syntax, bool isAllowLink, bool isAllowNewLine)
        {
            var result = syntax is SyntaxContent || syntax is SyntaxFont || syntax is SyntaxIgnore;
            if (isAllowLink)
            {
                result = result || syntax is SyntaxLink;
            }
            if (isAllowNewLine)
            {
                result = result || syntax is SyntaxNewLine;
            }
            return result;
        }

        /// <summary>
        /// Main entry for ParseTwo.
        /// </summary>
        internal static void ParseTwoMain(SyntaxBase owner, SyntaxBase syntax)
        {
            foreach (SyntaxBase item in syntax.List)
            {
                item.ParseTwo(owner);
            }
        }

        /// <summary>
        /// Main entry for ParseTwo with break.
        /// </summary>
        /// <param name="isOwnerNewChild">Returns true, if item is a child of ownerNew. If false, it is a child of owner.</param>
        internal static void ParseTwoMainBreak(SyntaxBase owner, SyntaxBase ownerNew, SyntaxBase syntax, Func<SyntaxBase, bool> isOwnerNewChild)
        {
            UtilDoc.Assert(ownerNew.Owner.Data == owner.Data);

            bool isOwnerNewChildLocal = true;
            foreach (DataDoc data in syntax.Data.ListGet())
            {
                SyntaxBase item = (SyntaxBase)data.Component(); 
                if (isOwnerNewChildLocal && isOwnerNewChild(item) == false)
                {
                    isOwnerNewChildLocal = false;
                }
                if (isOwnerNewChildLocal == false)
                {
                    // item is not child
                    item.ParseTwo(owner);
                }
                else
                {
                    // item is child
                    item.ParseTwo(ownerNew);
                }
            }
        }

        /// <summary>
        /// Main entry for ParseThree.
        /// </summary>
        internal static void ParseThreeMain(SyntaxBase owner, SyntaxBase syntax)
        {
            foreach (SyntaxBase item in syntax.List)
            {
                item.ParseThree(owner);
            }
        }

        internal enum OwnerEnum
        {
            None = 0,

            Page = 1,
             
            Paragraph = 2,
        }

        /// <summary>
        /// Main entry for ParseFour.
        /// </summary>
        internal static void ParseFourMain(SyntaxBase owner, SyntaxBase syntax)
        {
            foreach (SyntaxBase item in syntax.List)
            {
                item.ParseFour(owner);
            }
        }


        /// <summary>
        /// Main entry for ParseHtml.
        /// </summary>
        internal static void ParseHtmlMain(HtmlBase owner, SyntaxBase syntax)
        {
            foreach (SyntaxBase item in syntax.List)
            {
                item.ParseHtml(owner);
            }
        }

        /// <summary>
        /// Parse md token between tokenBegin and tokenEnd.
        /// </summary>
        internal virtual void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {

        }

        /// <summary>
        /// Override this method to custom transform syntax tree ParseOne into ParseTwo.
        /// </summary>
        internal virtual void ParseTwo(SyntaxBase owner)
        {
            if (owner.Data.Registry.SyntaxRegistry.SchemaTypeList.TryGetValue(GetType(), out var schemaTypeList))
            {
                var ownerNew = owner.Data.Registry.SyntaxRegistry.TypeList[GetType()].Create(owner, this);
                ParseTwoMainBreak(owner, ownerNew, this, (syntax) => schemaTypeList.Contains(syntax.GetType()));
            }
            else
            {
                var syntax = Data.Registry.SyntaxRegistry.TypeList[GetType()].Create(owner, this);
                ParseTwoMain(syntax, this);
            }
        }

        /// <summary>
        /// Override this method to custom transform syntax tree ParseTwo into ParseThree.
        /// </summary>
        internal virtual void ParseThree(SyntaxBase owner)
        {
            var ownerLocal = owner;
            var registry = Data.Registry.SyntaxRegistry;
            var ownerTypeList = registry.SchemaOwnerTypeList[GetType()];
            if (!ownerTypeList.Contains(owner.GetType()))
            {
                // Create new owner paragraph if content (this) is directly on page (owner).
                var ownerTypeDefault = ownerTypeList.First();
                ownerLocal = registry.TypeList[ownerTypeDefault].Create(owner, this);
                ownerLocal.Data.IsCreateNew = true;
            }
            var syntax = registry.TypeList[GetType()].Create(ownerLocal, this);
            ParseThreeMain(syntax, this);
        }


        /// <summary>
        /// Override this method to custom transform syntax tree ParseTwo into ParseThree.
        /// </summary>
        internal virtual void ParseFour(SyntaxBase owner)
        {
            SyntaxBase syntaxLast = null;
            if (owner.Data.ListCount() > 0)
            {
                syntaxLast = (SyntaxBase)owner.Data.List.Last().Component();
            }

            SyntaxBase syntaxLocal;
            // For example this paragraph and previous paragraph were both inserted (IsCreateNew).
            var isPrevious = syntaxLast?.IsCreateNew == true && IsCreateNew && syntaxLast.GetType() == GetType();
            if (isPrevious)
            {
                // Do not create new syntax but take previous one.
                syntaxLocal = syntaxLast;
            }
            else
            {
                var ownerLocal = owner;
                // For example previous paragraph has been inserted (IsCreateNew) and this is comment.
                isPrevious = syntaxLast?.IsCreateNew == true && IsCreateNew == false && syntaxLast.GetType() != GetType();
                if (isPrevious)
                {
                    // Check if for example this comment can be added to prevois paragraph.
                    if (Data.Registry.SyntaxRegistry.SchemaOwnerTypeList[GetType()].Contains(syntaxLast.GetType()))
                    {
                        ownerLocal = syntaxLast;
                    }
                }
                // Create new syntax
                syntaxLocal = Data.Registry.SyntaxRegistry.TypeList[GetType()].Create(ownerLocal, this);
                syntaxLocal.Data.IsCreateNew = IsCreateNew;
            }

            ParseFourMain(syntaxLocal, this);
        }

        /// <summary>
        /// Override this method to custom transform syntax tree ParseTwo into html.
        /// </summary>
        internal virtual void ParseHtml(HtmlBase owner)
        {
            ParseHtmlMain(owner, this);
        }
    }

    /// <summary>
    /// Syntax tree root.
    /// </summary>
    internal class SyntaxDoc : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxDoc(Registry registry)
            : base(registry)
        {

        }

        public SyntaxDoc(Component owner)
            : base(owner)
        {

        }
    }

    internal class SyntaxPage : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxPage(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxPage(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxPage(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {
            UtilDoc.Assert(owner is SyntaxDoc);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxDoc>();
        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxPage(owner, syntax);
        }
    }

    internal class SyntaxComment : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxComment(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxComment(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxComment(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxParagraph>();
            result.AddOwner<SyntaxPage>();
            result.AddOwner<SyntaxContent>();
            result.AddOwner<SyntaxBullet>();
            result.AddOwner<SyntaxTitle>();
        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxComment(owner, syntax);
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            if (tokenBegin is MdComment commentBegin && !commentBegin.IsCommentEnd)
            {
                Component component = tokenBegin;
                while (Next(ref component, tokenEnd))
                {
                    if (component is MdComment commentEnd && commentEnd.IsCommentEnd)
                    {
                        new SyntaxComment(owner, commentBegin, commentEnd);
                        break;
                    }
                }
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            new HtmlComment(owner, this);
        }
    }

    internal class SyntaxTitle : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxTitle(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxTitle(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxTitle(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {
            UtilDoc.Assert(owner is SyntaxPage);
        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxTitle(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxPage>();
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            if (ParseOneIsNewLine<MdTitle>(tokenBegin, tokenEnd, out MdSpace tokenSpace, out var token))
            {
                // Ignore leading space
                if (tokenSpace != null)
                {
                    new SyntaxIgnore(owner, tokenSpace);
                }

                var title = new SyntaxTitle(owner, token, tokenEnd);
                new SyntaxIgnore(title, token);

                // Ignore space after title
                if (token.Next(tokenEnd) is MdSpace space)
                {
                    new SyntaxIgnore(title, space);
                }

                ParseOneMain(title, this);
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            var title = new HtmlTitle(owner, this);

            ParseHtmlMain(title, this);
        }
    }

    internal class SyntaxBullet : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxBullet(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxBullet(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxBullet(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxBullet(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxPage>();
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            if (ParseOneIsNewLine<MdBullet>(tokenBegin, tokenEnd, out MdSpace tokenSpace, out var token))
            {
                // Space after star
                if (token.Next(tokenEnd) is MdSpace)
                {
                    // Ignore leading space
                    if (tokenSpace != null)
                    {
                        new SyntaxIgnore(owner, tokenSpace);
                    }

                    var bullet = new SyntaxBullet(owner, token, tokenEnd);
                    new SyntaxIgnore(bullet, token);

                    // Ignor space after bullet
                    if (token.Next(tokenEnd) is MdSpace space)
                    {
                        new SyntaxIgnore(bullet, space);
                    }

                    ParseOneMain(bullet, this);
                }
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            var bulletList = owner.Data.List?.Last().Component() as HtmlBulletList;
            if (bulletList == null)
            {
                bulletList = new HtmlBulletList(owner);
            }
            var bulletItem = new HtmlBulletItem(bulletList, this);

            ParseHtmlMain(bulletItem, this);
        }
    }

    internal class SyntaxCode : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxCode(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxCode(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd, string codeLanguage)
            : base(owner, tokenBegin, tokenEnd)
        {
            Data.CodeLanguage = codeLanguage;
        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxCode(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {
            Data.CodeLanguage = ((SyntaxCode)syntax).CodeLanguage;
            Data.CodeText = ((SyntaxCode)syntax).CodeText;
        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxCode(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxPage>();
        }

        public string CodeLanguage => Data.CodeLanguage;

        public string CodeText
        {
            get => Data.CodeText;
            set => Data.CodeText = value;
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            if (ParseOneIsNewLine<MdCode>(tokenBegin, tokenEnd, out MdSpace tokenSpace, out var codeBegin))
            {
                MdCode codeEnd = null;
                MdTokenBase next = codeBegin;
                while (Next(ref next, tokenEnd))
                {
                    if (next is MdCode tokenCode)
                    {
                        codeEnd = tokenCode;
                        break;
                    }
                }

                if (codeEnd != null)
                {
                    // Code language
                    MdContent codeLanguage = null;
                    if (codeBegin.Next(tokenEnd) is MdContent content)
                    {
                        codeLanguage = content;
                    }

                    // Next
                    next = codeLanguage != null ? codeLanguage : codeBegin;

                    // Code text
                    MdTokenBase codeTextBegin = null;
                    MdTokenBase codeTextEnd = null;
                    while (Next(ref next, tokenEnd))
                    {
                        if (next == codeEnd)
                        {
                            break;
                        }
                        if (codeTextBegin == null)
                        {
                            codeTextBegin = next;
                            codeTextEnd = next;
                        }
                        else
                        {
                            codeTextEnd = next;
                        }
                    }

                    // Code text exists
                    if (codeTextBegin != null)
                    {
                        // Ignore leading space
                        if (tokenSpace != null)
                        {
                            new SyntaxIgnore(owner, tokenSpace);
                        }

                        // Create code syntax
                        var code = new SyntaxCode(owner, codeBegin, codeEnd, codeLanguage?.Text);

                        // Ignore code language
                        new SyntaxIgnore(code, codeBegin);
                        if (codeLanguage != null)
                        {
                            new SyntaxIgnore(code, codeLanguage);
                        }

                        // Ignore code text
                        var codeText = new SyntaxIgnore(code, codeTextBegin, codeTextEnd);

                        code.CodeText = codeText.Text;

                        // Ignore code end token
                        new SyntaxIgnore(code, codeEnd);
                    }
                }
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            new HtmlCode(owner, this);
        }
    }

    internal class SyntaxFont : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxFont(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxFont(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxFont(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxFont(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxParagraph>();
            result.AddOwner<SyntaxTitle>();
            result.AddOwner<SyntaxBullet>();
            result.AddOwner<SyntaxPage>(false);
            result.AddOwner<SyntaxCustomNote>(false);
        }

        public MdFontEnum FontEnum => ((MdFont)TokenBegin).FontEnum;

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            if (tokenBegin is MdFont tokenFontBegin)
            {
                var next = tokenBegin;
                while (Next(ref next, tokenEnd))
                {
                    if (next is MdFont || next is MdNewLine)
                    {
                        break;
                    }
                }
                if (next != tokenBegin && next is MdFont tokenFontEnd && tokenFontEnd.FontEnum == tokenFontBegin.FontEnum)
                {
                    var syntaxFont = new SyntaxFont(owner, tokenBegin, next);
                    new SyntaxIgnore(syntaxFont, tokenBegin);
                    ParseOneMain(syntaxFont, this);
                }
                else
                {
                    if (owner is SyntaxFont syntaxFont)
                    {
                        new SyntaxIgnore(syntaxFont, tokenBegin);
                    }
                    else
                    {
                        new SyntaxContent(owner, tokenBegin, tokenBegin);
                    }
                }
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            var font = owner;
            bool isBegin = !(Owner is SyntaxFont);

            if (isBegin)
            {
                font = new HtmlFont(owner, this);
            }

            ParseHtmlMain(font, this);
        }
    }

    internal class SyntaxLink : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxLink(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxLink(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd, string link, string linkText)
            : base(owner, tokenBegin, tokenEnd)
        {
            Data.Link = link;
            Data.LinkText = linkText;
        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxLink(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {
            Data.Link = ((SyntaxLink)syntax).Link;
            Data.LinkText = ((SyntaxLink)syntax).LinkText;
        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxLink(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxParagraph>();
            // result.AddOwner<SyntaxTitle>(); // No link in title
            result.AddOwner<SyntaxBullet>();
        }

        public string Link => Data.Link;

        public string LinkText => Data.LinkText;

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            // For example http://
            if (tokenBegin is MdLink)
            {
                if (ParseOneIsLink(tokenBegin, tokenEnd, out var linkEnd, out string link))
                {
                    new SyntaxLink(owner, tokenBegin, linkEnd, link, link);
                }
            }

            // For example []()
            if (tokenBegin is MdBracket bracketSquare && bracketSquare.TextBracket == "[")
            {
                var next = tokenBegin.Next(tokenEnd);
                ParseOneIsLinkText(next, tokenEnd, out var linkTextEnd, out string linkText);
                if (linkText != null)
                {
                    next = linkTextEnd.Next(tokenEnd);
                }
                if (next is MdBracket bracketSquareEnd && bracketSquareEnd.TextBracket == "]")
                {
                    next = next.Next(tokenEnd);
                    if (next is MdBracket bracketRound && bracketRound.TextBracket == "(")
                    {
                        next = next.Next(tokenEnd);
                        if (ParseOneIsLinkText(next, tokenEnd, out var linkEnd, out string link))
                        {
                            next = linkEnd.Next(tokenEnd);
                            if (next is MdBracket bracketRoundEnd && bracketRoundEnd.TextBracket == ")")
                            {
                                if (linkText == null)
                                {
                                    linkText = link;
                                }
                                new SyntaxLink(owner, tokenBegin, next, link, linkText);
                            }
                        }
                    }
                }
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            new HtmlLink(owner, this);
        }
    }

    internal class SyntaxImage : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxImage(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxImage(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd, string link, string linkText)
            : base(owner, tokenBegin, tokenEnd)
        {
            Data.Link = link;
            Data.LinkText = linkText;
        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxImage(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {
            Data.Link = ((SyntaxImage)syntax).Link;
            Data.LinkText = ((SyntaxImage)syntax).LinkText;
        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxImage(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxPage>();
        }

        public string Link => Data.Link;

        public string LinkText => Data.LinkText;

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            // For example ![]()
            if (tokenBegin is MdImage)
            {
                MdTokenBase next = tokenBegin.Next(tokenEnd);
                ParseOneIsLinkText(next, tokenEnd, out var linkTextEnd, out string linkText);
                if (linkText != null)
                {
                    next = linkTextEnd.Next(tokenEnd);
                }
                if (next is MdBracket bracketSquareEnd && bracketSquareEnd.TextBracket == "]")
                {
                    next = next.Next(tokenEnd);
                    if (next is MdBracket bracketRound && bracketRound.TextBracket == "(")
                    {
                        next = next.Next(tokenEnd);
                        if (ParseOneIsLink(next, tokenEnd, out var linkEnd, out var link))
                        {
                            if (linkEnd.Next(tokenEnd) is MdBracket bracketRoundEnd && bracketRoundEnd.Text == ")")
                            {
                                new SyntaxImage(owner, tokenBegin, bracketRoundEnd, link, linkText != null ? linkText : link);
                            }
                        }
                    }
                }
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            new HtmlImage(owner, this);
        }
    }

    internal class SyntaxContent : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxContent(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxContent(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxContent(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxContent(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxParagraph>();
            result.AddOwner<SyntaxPage>(false);
            result.AddOwner<SyntaxTitle>();
            result.AddOwner<SyntaxBullet>();
            result.AddOwner<SyntaxFont>();
            result.AddOwner<SyntaxCustomNote>(false);
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            bool isFind = false;
            if (tokenBegin is MdContent)
            {
                var contentEnd = tokenBegin;

                if (tokenBegin.Next(tokenEnd) is MdSpace)
                {
                    var next = tokenBegin.Next(tokenEnd).Next(tokenEnd) as MdContent;
                    if (next != null)
                    {
                        contentEnd = next;
                    }
                }

                new SyntaxContent(owner, tokenBegin, contentEnd);
                isFind = true;
            }

            if (tokenBegin is MdSpace)
            {
                var next = tokenBegin.Next(tokenEnd);
                if (next is not MdNewLine && next is not MdComment)
                {
                    new SyntaxContent(owner, tokenBegin, tokenBegin);
                    isFind = true;
                }
            }

            if (!isFind)
            {
                new SyntaxContent(owner, tokenBegin, tokenBegin);
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            new HtmlContent(owner, this);
        }
    }

    internal class SyntaxParagraph : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxParagraph(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxParagraph(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxParagraph(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {
            UtilDoc.Assert(owner is SyntaxPage || owner is SyntaxCustomNote);
        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxParagraph(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxPage>();
            result.AddOwner<SyntaxCustomNote>();
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            if (tokenBegin is MdParagraph)
            {
                var paragraph = new SyntaxParagraph(owner, tokenBegin, tokenEnd);
                // Ignore paragrpah token
                new SyntaxIgnore(paragraph, tokenBegin);
                ParseOneMain(paragraph, this);
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            var paragraph = new HtmlParagraph(owner, this);

            ParseHtmlMain(paragraph, this);
        }
    }

    internal class SyntaxNewLine : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxNewLine(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxNewLine(SyntaxBase owner, MdTokenBase token)
            : base(owner, token, token)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxNewLine(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxNewLine(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxParagraph>();
            result.AddOwner<SyntaxPage>(false);
            // result.AddOwner<SyntaxTitle>(); // New line in title is the end of title.
            result.AddOwner<SyntaxBullet>();
            result.AddOwner<SyntaxCustomNote>();
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            if (tokenBegin is MdNewLine)
            {
                UtilDoc.Assert(tokenBegin.Next(tokenEnd) is not MdNewLine); // Detected by SyntaxParagraph.
                new SyntaxNewLine(owner, tokenBegin);
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            // No html for NewLine token.
        }
    }

    internal class SyntaxOmit : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxOmit(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxOmit(SyntaxBase owner, MdTokenBase token)
            : base(owner, token, token)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxOmit(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxOmit(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxOmit(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxPage>();
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            if (tokenBegin.IsOmit)
            {
                new SyntaxOmit(owner, tokenBegin);
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            // No html
        }
    }

    internal class SyntaxIgnore : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxIgnore(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxIgnore(SyntaxBase owner, MdTokenBase token)
            : base(owner, token, token)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxIgnore(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxIgnore(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxIgnore(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxParagraph>();
            result.AddOwner<SyntaxTitle>();
            result.AddOwner<SyntaxBullet>();
            result.AddOwner<SyntaxFont>();
            result.AddOwner<SyntaxCustomNote>();
            result.AddOwner<SyntaxCode>();
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            // No parser found for tokenBegin!
            throw new Exception("Syntax unknown!");
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            // No html
        }
    }

    /// <summary>
    /// Base class for custom syntax. For example (Message Type="Warning")Do not delete!(Message)
    /// </summary>
    internal abstract class SyntaxCustomBase : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxCustomBase(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxCustomBase(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxCustomBase(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        /// <summary>
        /// Returns true, if custom command starting at new line has been found.
        /// </summary>
        internal static bool ParseOneIsCustom(MdTokenBase tokenBegin, MdTokenBase tokenEnd, string commandName, out MdBracket token, out List<MdTokenBase> ignoreList)
        {
            var result = false;
            ignoreList = null;
            if (ParseOneIsNewLine(tokenBegin, tokenEnd, out var space, out token))
            {
                if (token.TextBracket == "(")
                {
                    if (space == null)
                    {
                        if (token.Next(tokenEnd) is MdContent content)
                        {
                            if (content.Text == commandName)
                            {
                                if (content.Next(tokenEnd) is MdBracket bracketEnd)
                                {
                                    if (bracketEnd.Text == ")")
                                    {
                                        ignoreList = new List<MdTokenBase>();
                                        ignoreList.Add(content);
                                        ignoreList.Add(bracketEnd);
                                        result = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns true, if custom command (with end block) starting at new line has been found.
        /// </summary>
        internal static bool ParseOneIsCustom(MdTokenBase tokenBegin, MdTokenBase tokenEnd, string commandName, out MdBracket tokenBlockBegin, out List<MdTokenBase> ignoreBlockBeginList, out MdBracket tokenBlockEnd, out List<MdTokenBase> ignoreBlockEndList)
        {
            bool result = false;
            tokenBlockEnd = null;
            ignoreBlockEndList = null;
            if (ParseOneIsCustom(tokenBegin, tokenEnd, commandName, out tokenBlockBegin, out ignoreBlockBeginList))
            {
                var next = ignoreBlockBeginList.Last();
                while (Next(ref next, tokenEnd))
                {
                    if (ParseOneIsCustom(next, tokenEnd, commandName, out tokenBlockEnd, out ignoreBlockEndList))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
    }

    internal class SyntaxCustomNote : SyntaxCustomBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxCustomNote(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxCustomNote(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxCustomNote(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxCustomNote(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxPage>();
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            if (ParseOneIsCustom(tokenBegin, tokenEnd, "Note", out var tokenBlockBegin, out var tokenBlockBeginList, out var tokenBlockEnd, out var tokenBlockEndList))
            {
                var note = new SyntaxCustomNote(owner, tokenBlockBegin, tokenBlockEnd.Previous(tokenBegin));
                
                new SyntaxIgnore(note, tokenBlockBegin);
                foreach (var item in tokenBlockBeginList)
                {
                    new SyntaxIgnore(note, item);
                }

                ParseOneMain(note, this);

                // Shrink custom note parsed range if it contains invalid syntax like for example bullet.
                MdTokenBase tokenEndShrink = null;
                var registry = Data.Registry.SyntaxRegistry;
                var typeList = registry.SchemaTypeList[note.GetType()];
                foreach (SyntaxBase item in note.ListAll())
                {
                    if (item.Data != note.Data)
                    {
                        if (!typeList.Contains(item.GetType()))
                        {
                            var itemPrevious = item.Previous<SyntaxBase>(null);
                            tokenEndShrink = itemPrevious.TokenEnd;
                            break;
                        }
                    }
                }

                // Shrink custom note range.
                if (tokenEndShrink != null)
                {
                    note.Remove();
                    note = new SyntaxCustomNote(owner, tokenBlockBegin, tokenEndShrink);

                    new SyntaxIgnore(note, tokenBlockBegin);
                    foreach (var item in tokenBlockBeginList)
                    {
                        new SyntaxIgnore(note, item);
                    }

                    // Parse again with new range.
                    ParseOneMain(note, this);

                    tokenBlockEnd.IsOmit = true;
                    foreach (var item in tokenBlockEndList)
                    {
                        item.IsOmit = true;
                    }
                }
                else
                {
                    note.TokenEndSet(tokenBlockEndList.Last());

                    new SyntaxIgnore(note, tokenBlockEnd);
                    foreach (var item in tokenBlockEndList)
                    {
                        new SyntaxIgnore(note, item);
                    }
                }
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            var note = new HtmlCustomNote(owner, this);

            ParseHtmlMain(note, this);
        }
    }

    /// <summary>
    /// Custom syntax for page break.
    /// </summary>
    internal class SyntaxPageBreak : SyntaxBase
    {
        /// <summary>
        /// Constructor registry, factory mode.
        /// </summary>
        public SyntaxPageBreak(Registry registry)
            : base(registry)
        {

        }

        /// <summary>
        /// Constructor ParseOne.
        /// </summary>
        public SyntaxPageBreak(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
            : base(owner, tokenBegin, tokenEnd)
        {

        }

        /// <summary>
        /// Constructor ParseTwo and ParseThree.
        /// </summary>
        public SyntaxPageBreak(SyntaxBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        protected internal override SyntaxBase Create(SyntaxBase owner, SyntaxBase syntax)
        {
            return new SyntaxPageBreak(owner, syntax);
        }

        internal override void RegistrySchema(RegistrySchemaResult result)
        {
            result.AddOwner<SyntaxDoc>();
        }

        internal override void ParseOne(SyntaxBase owner, MdTokenBase tokenBegin, MdTokenBase tokenEnd)
        {
            // Detect (Page)
            if (ParseOneIsNewLine<MdBracket>(tokenBegin, tokenEnd, out var tokenSpace, out var token))
            {
                if (token.TextBracket == "(")
                {
                    if (token.Next(tokenEnd) is MdContent content && token.Next(tokenEnd)?.Next(tokenEnd) is MdBracket bracketEnd)
                    {
                        if (bracketEnd.TextBracket == ")")
                        {
                            if (content.Text == "Page")
                            {
                                // Ignore leading space
                                if (tokenSpace != null)
                                {
                                    new SyntaxIgnore(owner, tokenSpace);
                                }
                                var pageBreak = new SyntaxPageBreak(owner, token, tokenEnd);
                                new SyntaxIgnore(pageBreak, token);
                                new SyntaxIgnore(pageBreak, content);
                                new SyntaxIgnore(pageBreak, bracketEnd);
                                ParseOneMain(pageBreak, this);
                            }
                        }
                    }
                }
            }
        }

        internal override void ParseHtml(HtmlBase owner)
        {
            var page = new HtmlPage(owner, this);

            ParseHtmlMain(page, this);
        }
    }

    /// <summary>
    /// Base class for html syntax tree.
    /// </summary>
    internal abstract class HtmlBase : Component
    {
        /// <summary>
        /// Constructor for Doc.
        /// </summary>
        public HtmlBase(Component owner)
            : base(owner)
        {

        }

        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlBase(HtmlBase owner, SyntaxBase syntax)
            : base(owner)
        {
            UtilDoc.Assert(owner.Data.Registry.ParseEnum == ParseEnum.ParseHtml);

            Data.SyntaxId = Registry.ReferenceSet(syntax);
        }

        public SyntaxBase Syntax => Data.Registry.ReferenceGet<SyntaxBase>(Data.SyntaxId);

        internal string Render()
        {
            var result = new StringBuilder();
            Render(result);
            return UtilDoc.StringNull(result.ToString());
        }

        internal void Render(StringBuilder result)
        {
            RenderBegin(result);
            RenderContent(result);
            RenderEnd(result);
        }

        internal virtual void RenderBegin(StringBuilder result)
        {

        }

        internal virtual void RenderContent(StringBuilder result)
        {
            foreach (HtmlBase item in List)
            {
                item.Render(result);
            }
        }

        internal virtual void RenderEnd(StringBuilder result)
        {

        }
    }

    /// <summary>
    /// Html tree root.
    /// </summary>
    internal class HtmlDoc : HtmlBase
    {
        public HtmlDoc(Component owner)
            : base(owner)
        {

        }
    }

    internal class HtmlPage : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlPage(HtmlBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        internal override void RenderBegin(StringBuilder result)
        {
            // result.Append("<html><head></head><body>");
        }

        internal override void RenderEnd(StringBuilder result)
        {
            // result.Append("</body></html>");
        }
    }

    internal class HtmlComment : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlComment(HtmlBase owner, SyntaxComment syntax)
            : base(owner, syntax)
        {

        }

        internal override void RenderContent(StringBuilder result)
        {
            result.Append(Syntax.Text);
        }
    }

    internal class HtmlTitle : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlTitle(HtmlBase owner, SyntaxTitle syntax)
            : base(owner, syntax)
        {

        }

        internal override void RenderBegin(StringBuilder result)
        {
            result.Append("<h1>");
        }

        internal override void RenderEnd(StringBuilder result)
        {
            result.Append("</h1>");
        }
    }

    internal class HtmlParagraph : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlParagraph(HtmlBase owner, SyntaxParagraph syntax)
            : base(owner, syntax)
        {

        }

        internal override void RenderBegin(StringBuilder result)
        {
            result.Append("<p>(p)");
        }

        internal override void RenderEnd(StringBuilder result)
        {
            result.Append("(/p)</p>");
        }
    }

    internal class HtmlBulletList : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlBulletList(HtmlBase owner)
            : base(owner, null)
        {

        }

        internal override void RenderBegin(StringBuilder result)
        {
            result.Append("<ul>");
        }

        internal override void RenderEnd(StringBuilder result)
        {
            result.Append("</ul>");
        }
    }

    internal class HtmlBulletItem : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlBulletItem(HtmlBulletList owner, SyntaxBullet syntax)
            : base(owner, syntax)
        {

        }

        internal override void RenderBegin(StringBuilder result)
        {
            result.Append("<li>");
        }

        internal override void RenderEnd(StringBuilder result)
        {
            result.Append("</li>");
        }
    }

    internal class HtmlFont : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlFont(HtmlBase owner, SyntaxFont syntax)
            : base(owner, syntax)
        {

        }

        public new SyntaxFont Syntax => (SyntaxFont)base.Syntax;

        internal override void RenderBegin(StringBuilder result)
        {
            switch (Syntax.FontEnum)
            {
                case MdFontEnum.Bold:
                    result.Append("<strong>");
                    break;
                case MdFontEnum.Italic:
                    result.Append("<i>");
                    break;
                default:
                    throw new Exception("Enum unknown!");
            }
        }

        internal override void RenderEnd(StringBuilder result)
        {
            switch (Syntax.FontEnum)
            {
                case MdFontEnum.Bold:
                    result.Append("</strong>");
                    break;
                case MdFontEnum.Italic:
                    result.Append("</i>");
                    break;
                default:
                    throw new Exception("Enum unknown!");
            }
        }
    }

    internal class HtmlLink : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlLink(HtmlBase owner, SyntaxLink syntax)
            : base(owner, syntax)
        {

        }

        public new SyntaxLink Syntax => (SyntaxLink)base.Syntax;

        internal override void RenderContent(StringBuilder result)
        {
            result.Append($"<a href=\"{ Syntax.Link }\">{ Syntax.LinkText }</a>");
        }
    }

    internal class HtmlImage : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlImage(HtmlBase owner, SyntaxImage syntax)
            : base(owner, syntax)
        {

        }

        public new SyntaxImage Syntax => (SyntaxImage)base.Syntax;

        internal override void RenderContent(StringBuilder result)
        {
            if (Syntax.LinkText == null)
            {
                result.Append($"<a src=\"{ Syntax.Link }\" />{ Syntax.LinkText }</a>");
            }
            else
            {
                result.Append($"<img src=\"{ Syntax.Link }\" alt=\"{ Syntax.LinkText }\" />");
            }
        }
    }

    internal class HtmlCode : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlCode(HtmlBase owner, SyntaxCode syntax)
            : base(owner, syntax)
        {

        }

        public new SyntaxCode Syntax => (SyntaxCode)base.Syntax;

        internal override void RenderContent(StringBuilder result)
        {
            result.Append(string.Format("<pre><code class=\"{0}\">", "language-" + Syntax.CodeLanguage));
            result.Append(Syntax.CodeText);
            result.Append("</code></pre>");
        }
    }

    internal class HtmlCustomNote : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlCustomNote(HtmlBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        internal override void RenderBegin(StringBuilder result)
        {
            result.Append("<article class=\"message is-warning\"><div class=\"message-body\">");
        }

        internal override void RenderEnd(StringBuilder result)
        {
            result.Append("</div></article>");
        }
    }

    internal class HtmlContent : HtmlBase
    {
        /// <summary>
        /// Constructor parse html.
        /// </summary>
        public HtmlContent(HtmlBase owner, SyntaxBase syntax)
            : base(owner, syntax)
        {

        }

        internal override void RenderContent(StringBuilder result)
        {
            result.Append(Syntax.Text);
        }
    }

    internal static class UtilDoc
    {
        public static void Debug()
        {
            string textMd = "(Note)\r\n# X5\r\n**Bold**\r\n(Note)";
            // textMd = "# Hello<!-- Comment -->World";
            // textMd = "Hello\r\nWorld\r\n* One\r\n* Two";
            // textMd = "Hello\r\n\r\nWorld";
            // textMd = "Hello<!-- Comment -->";
            //textMd = "Hello<!-- Comment -->World";
            // textMd = "**Bold**World";
            // textMd = "(Note)\r\n\r\nA\r\n(Note)";
            // textMd = "(Note)\r\n\r\n* A\r\n(Note)";
            // textMd = "(Note)\r\n\r\nHello\r\n\r\n# A\r\n# B\r\n(Note)";
            // textMd = "(Note)Hello\r\n# A\r\n(Note)";
            // textMd = "(Note)\r\n\r\n* A\r\n(Note)";

            // Doc
            var appDoc = new AppDoc();
            new MdPage(appDoc.MdDoc, textMd);
            string exceptionText = null;
            try
            {
                appDoc.Parse();
            }
            catch (Exception exception)
            {
                exceptionText = exception.Message;
            }

            // Serialize, deserialize
            appDoc.Serialize(out string json);
            Component.Deserialize<AppDoc>(json);

            // Write file Debug.txt
            TextDebugWriteToFile(appDoc, exceptionText);
        }

        private static string TextDebug(string text)
        {
            return text?.Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private static void TextDebug(Component component, int level, StringBuilder result)
        {
            for (int i = 0; i < level; i++)
            {
                result.Append("  ");
            }

            string syntaxIdText = null;
            if (component is SyntaxBase syntaxId && syntaxId.Data.SyntaxId != null)
            {
                syntaxIdText = "->" + string.Format("{0:000}", syntaxId.Data.SyntaxId);
            }

            result.Append("-(" + component.GetType().Name + " " + string.Format("{0:000}", component.Data.Id) + syntaxIdText + ")");

            // Token
            if (component is MdTokenBase token)
            {
                result.Append(" Text=\"" + TextDebug(token.Text) + "\";");
            }
            // Syntax
            if (component is SyntaxBase syntax)
            {
                if (syntax is SyntaxDoc doc)
                {
                    if (doc.OwnerFind<AppDoc>().SyntaxDocOne.Data == doc.Data)
                    {
                        result.Append(" ParseOne");
                    }
                    if (doc.OwnerFind<AppDoc>().SyntaxDocTwo.Data == doc.Data)
                    {
                        result.Append(" ParseTwo (Break)");
                    }
                    if (doc.OwnerFind<AppDoc>().SyntaxDocThree.Data == doc.Data)
                    {
                        result.Append(" ParseThree (IsCreateNew Create)");
                    }
                    if (doc.OwnerFind<AppDoc>().SyntaxDocFour.Data == doc.Data)
                    {
                        result.Append(" ParseFour (IsCreateNew Merge)");
                    }
                }
                else
                {
                    result.Append(" Text=\"" + TextDebug(syntax.Text) + "\";");
                    if (syntax.IsCreateNew)
                    {
                        result.Append(" IsCreateNew;");
                    }
                }
            }
            // Html
            if (component is HtmlBase syntaxHtml)
            {
                result.Append(" Text=\"" + TextDebug(syntaxHtml.Syntax?.Text) + "\";");
            }

            result.AppendLine();
            foreach (var item in component.List)
            {
                TextDebug(item, level + 1, result);
            }
        }

        public static string TextDebug(Component component)
        {
            StringBuilder result = new StringBuilder();
            TextDebug(component, 0, result);
            return StringNull(result.ToString());
        }

        public static void TextDebugWriteToFile(AppDoc appDoc, string exceptionText = null)
        {
            string textMd = ((MdPage)appDoc.MdDoc.List.First()).Text;
            string result = TextDebug(appDoc);
            string textHtml = appDoc.HtmlDoc.Render();

            if (exceptionText != null)
            {
                result += "\r\n\r\n" + exceptionText;
            }

            result += "\r\n\r\n" + "Md:\r\n";
            result += textMd;

            result += "\r\n\r\n" + "Html:\r\n";
            result += textHtml;

            string textMdEscape = textMd.Replace("\r", "\\r").Replace("\n", "\\n");
            string textHtmlEscape = textHtml?.Replace("\"", @"\""");
            textHtmlEscape = textHtmlEscape?.Replace("\r", "\\r").Replace("\n", "\\n");
            string textCSharp = string.Format("list.Add(new Item (( TextMd = \"{0}\", TextHtml = \"{1}\" )));", textMdEscape, textHtmlEscape).Replace("((", "{").Replace("))", "}");

            result += "\r\n\r\n" + "CSharp:\r\n";
            result += textCSharp;

            File.WriteAllText(@"C:\Temp\Debug.txt", result);
            // File.WriteAllText(@"C:\Temp\Debug.html", textHtml);
        }

        public static void Assert(bool isAssert, string exceptionText)
        {
            if (!isAssert)
            {
                throw new Exception(exceptionText);
            }
        }

        public static void Assert(bool isAssert)
        {
            Assert(isAssert, "Assert!");
        }

        public static bool IsSubclassOf(Type type, Type typeBase)
        {
            return type == typeBase || type.IsSubclassOf(typeBase);
        }

        public static string StringNull(string text)
        {
            return text == "" ? null : text;
        }
    }
}
