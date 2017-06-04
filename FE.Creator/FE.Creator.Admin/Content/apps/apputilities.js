function initializeckEditor(editorid) {
    var editor = CKEDITOR.replace(editorid, {
        // Define the toolbar: http://docs.ckeditor.com/#!/guide/dev_toolbar
        // The standard preset from CDN which we used as a base provides more features than we need.
        // Also by default it comes with a 2-line toolbar. Here we put all buttons in a single row.
        toolbar: [
            { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteFromWord', '-', 'Undo', 'Redo'] },
            { name: 'basicstyles', items: ['Bold', 'Italic'] },
            { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl', 'Blockquote', 'align', 'bidi'] },
            { name: 'links', items: ['Link', 'Unlink'] },
            { name: 'insert', items: ['CodeSnippet', 'Image', 'Mathjax'] },
            { name: 'basicstyles', items: ['basicstyles', 'cleanup'] },
            { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
            { name: 'colors', items: ['TextColor', 'BGColor'] },
            { name: 'document', items: ['Source', 'Print'] },
            { name: 'tools', items: ['Maximize', 'ShowBlocks'] },
        ],

        // Since we define all configuration options here, let's instruct CKEditor to not load config.js which it does by default.
        // One HTTP request less will result in a faster startup time.
        // For more information check http://docs.ckeditor.com/#!/api/CKEDITOR.config-cfg-customConfig
        customConfig: '',

        // Enabling extra plugins, available in the standard-all preset: http://ckeditor.com/presets-all
        extraPlugins: 'codesnippet,image2,imageresponsive,mathjax,uploadimage',

        //for build in upload manager.
        filebrowserUploadUrl: "/FileUpload/CKEditorUpload",

        //for uploadimage plugin to drag and drop images.
        imageUploadUrl: "/api/Files?forContent=true",
        // Remove the default image plugin because image2, which offers captions for images, was enabled above.
        removePlugins: 'image',

        // See http://docs.ckeditor.com/#!/api/CKEDITOR.config-cfg-mathJaxLib
        mathJaxLib: 'https://cdn.mathjax.org/mathjax/2.6-latest/MathJax.js?config=TeX-AMS_HTML',

        // See http://docs.ckeditor.com/#!/api/CKEDITOR.config-cfg-codeSnippet_theme
        codeSnippet_theme: 'github',

        /*********************** File management support ***********************/
        // In order to turn on support for file uploads, CKEditor has to be configured to use some server side
        // solution with file upload/management capabilities, like for example CKFinder.
        // For more information see http://docs.ckeditor.com/#!/guide/dev_ckfinder_integration

        // Uncomment and correct these lines after you setup your local CKFinder instance.
        // filebrowserBrowseUrl: 'http://example.com/ckfinder/ckfinder.html',
        // filebrowserUploadUrl: 'http://example.com/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Files',
        /*********************** File management support ***********************/

        // Make the editing area bigger than default.
        height: 461,

        // An array of stylesheets to style the WYSIWYG area.
        // Note: it is recommended to keep your own styles in a separate file in order to make future updates painless.
        contentsCss: ['/Content/adminlte-2.3.6/plugins/ckeditor/contents.css'],

        // Reduce the list of block elements listed in the Format dropdown to the most commonly used.
        format_tags: 'p;h1;h2;h3;pre',

        // Simplify the Image and Link dialog windows. The "Advanced" tab is not needed in most cases.
        removeDialogTabs: 'image:advanced;link:advanced;link:target',

        // Define the list of styles which should be available in the Styles dropdown list.
        // If the "class" attribute is used to style an element, make sure to define the style for the class in "mystyles.css"
        // (and on your website so that it rendered in the same way).
        // Note: by default CKEditor looks for styles.js file. Defining stylesSet inline (as below) stops CKEditor from loading
        // that file, which means one HTTP request less (and a faster startup).
        // For more information see http://docs.ckeditor.com/#!/guide/dev_styles
        stylesSet: [
            /* Inline Styles */
            { name: 'Marker', element: 'span', attributes: { 'class': 'marker' } },
            { name: 'Cited Work', element: 'cite' },
            { name: 'Inline Quotation', element: 'q' },

            /* Object Styles */
            {
                name: 'Special Container',
                element: 'div',
                styles: {
                    padding: '5px 10px',
                    background: '#eee',
                    border: '1px solid #ccc'
                }
            },
            {
                name: 'Compact table',
                element: 'table',
                attributes: {
                    cellpadding: '5',
                    cellspacing: '0',
                    border: '1',
                    bordercolor: '#ccc'
                },
                styles: {
                    'border-collapse': 'collapse'
                }
            },
            { name: 'Borderless Table', element: 'table', styles: { 'border-style': 'hidden', 'background-color': '#E6E6FA' } },
            { name: 'Square Bulleted List', element: 'ul', styles: { 'list-style-type': 'square' } },

            /* Widget Styles */
            { name: 'Illustration', type: 'widget', widget: 'image', attributes: { 'class': 'image-illustration' } },
            { name: 'Featured snippet', type: 'widget', widget: 'codeSnippet', attributes: { 'class': 'code-featured' } },
            { name: 'Featured formula', type: 'widget', widget: 'mathjax', attributes: { 'class': 'math-featured' } }
        ]
    });

    editor.on('fileUploadRequest', function (evt) {
        var fileLoader = evt.data.fileLoader,
         formData = new FormData(),
         xhr = fileLoader.xhr;
        xhr.open('post', fileLoader.uploadUrl, true);
        formData.append('upload', fileLoader.file, fileLoader.fileName);
        fileLoader.xhr.send(formData);

        // Prevented the default behavior.
        evt.stop();
    });
}

function highlightCode(selector) {
    $(selector).each(function(i, block) {
        hljs.highlightBlock(block);
    });
}