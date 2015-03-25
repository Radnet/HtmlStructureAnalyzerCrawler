using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class TagQTD
    { 
        //--------- TEXT & STYLERS--------------------------
        // Title of the document
        public int title    { get; set; }

        // header for a document or section
        public int header   { get; set; }

        // Heading
        public int h1       { get; set; }
        public int h2       { get; set; }
        public int h3       { get; set; }
        public int h4       { get; set; }
        public int h5       { get; set; }
        public int h6       { get; set; }

        //  Paragraph       
        public int p        { get; set; }

        // LineBreak
        public int br       { get; set; }

        // Footer
        public int footer   { get; set; }

        // Specifies independent, self-contained content
        public int article  {get; set; }

        // Some content aside from the content it is placed in
        public int aside    { get; set; }

        // Define font, color etc. for a text
        public int font     { get; set; }
        
        // Emphasized text
        public int em       { get; set; }

        // Strong Text
        public int strong   { get; set; }

        // Italic
        public int i        { get; set; }

        // UnderLine
        public int u        { get; set; }

        // Text that has been inserted into a document
        public int ins      { get; set; }

        // Text that has been deleted of a document
        public int del      { get; set; }

        // Marked Text
        public int mark     { get; set; }

        // Definition term
        public int dfn      { get; set; }

        // A piece of computer code
        public int code     { get; set; }

        // Sample output from a computer program
        public int samp     { get; set; }

        // Keyboard input
        public int kbd      { get; set; }

        // Variable
        public int var      { get; set; }

        // Pre formated Text
        public int pre      { get; set; }

        // Short Quotation
        public int q        { get; set; }

        // Small Text
        public int small    { get; set; }

        // Subscript Text
        public int sub      { get; set; }

        // Superscript Text
        public int sup      { get; set; }

        // Summary
        public int summary  { get; set; }

        // DateTime
        public int time     { get; set; }

        //--------- LINK --------------------------
        public int a        { get; set; }

        //--------- ADDRESS------------------------
        public int address  { get; set; }

        //--------- INTERACTION -------------------
        // Audio
        public int audio    { get; set; }

        // Tracks for media elements (<audio> and <video>)
        public int track { get; set; }
        
        //Button
        public int button   { get; set; }

        // Any kind of input
        public int input    { get; set; }

        // List of pre-defined options for an <input> element.
        public int datalist { get; set; }

        // Description list
        public int dl       { get; set; }

        // Terms in a Description List
        public int dt       { get; set; }

        // Description Term Value
        public int dd       { get; set; }

        // An option in a select list
        public int option   { get; set; }

        // Key-pair generator field used for forms
        public int keygen   { get; set; }

        // Result of a calculation
        public int output   { get; set; } 

        //---------- VISUAL ----------------------
        // Image
        public int img      { get; set; }

        // Specifies self-contained content, like illustrations, diagrams, photos, code listings, etc.
        public int figure   { get; set; }

        // Defines a caption for a <figure> element
        public int figcaption { get; set; }

        // Drawed Image
        public int canvas   { get; set; }

        // Define a client-side image-map
        public int map      { get; set; }

        // Scalar measurement within a known range
        public int meter    { get; set; }

        // Progress bar
        public int progress { get; set; }

        // Audio or Video source
        public int source   { get; set; }

        //--------- TABLE -----------------------
        // Table
        public int table    { get; set; }

        // Table cell
        public int td { get; set; }
        
        // Table cell header
        public int th { get; set; }

        // Table footer
        public int tfoot { get; set; }
        
        // Column
        public int col      { get; set; }

        // Group of one or more columns
        public int colgroup { get; set; }

        //------ PAGE ORGANIZERS ----------------
        //container for all the head elements
        public int head     { get; set; }

        // Page Section
        public int div      { get; set; }

        // Frame Group 
        public int frameset { get; set; }
 
        // Frame
        public int frame    { get; set; }

        // Link between a document and an external resource
        public int link     { get; set; }

        // Set of navigation links
        public int nav      { get; set; }

        //------ EXTERNAL APLICATION -----------
        public int embed    { get; set; }

        //----------- FORM ---------------------
        // Defines a form
        public int form     { get; set; }

        // Group related elements in a form
        public int fieldset { get; set; }
        
        // Field Set Title
        public int legend   { get; set; }

        // List item
        public int li       { get; set; }
        
        // Orded list
        public int ol       { get; set; }
        
        // Unorded list
        public int ul       { get; set; }

        // Defines a list/menu of commands
        public int menu     { get; set; }

        // Item of Menu
        public int menuitem { get; set; }

    }
}
