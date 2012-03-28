(function () {
    console.log('Tripmarket loaded!');


    var overlay = document.createElement('div');
    overlay.innerHTML = '<h1>test</h1>';
    overlay.style.cssText = 'position: fixed; top: 0; right:0; bottom: 0; left: 0; background-color: #F2F2F2; opacity: .6; z-index: 8675310; ';
    document.body.appendChild(overlay);

    var createActivityLink = document.createElement('a');
    createActivityLink.style.cssText = 'cursor: pointer;';
    createActivityLink.textContent = 'Create Activity';
    
    createActivityLink.onclick = function () {
        console.log('click');
        var src = '{siteRoot}Tripmarklet?url=' + encodeURIComponent(document.URL) + '&title=' + encodeURIComponent(document.title);
        //window.open(src);
        window.open(src,'name','height=255,width=250,toolbar=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=no ,modal=yes');
    };
    overlay.appendChild(createActivityLink);

    //    var id = 'triptitude_iframe';
    //    var existing = document.getElementById(id);
    //    if (existing)
    //        existing.parentNode.removeChild(existing);

    //    var i = document.createElement('iframe');
    //    i.setAttribute('id', id);
    //    i.style.position = 'fixed';
    //    i.style.zIndex = 999999998;
    //    i.style.width = '100%';
    //    i.style.height = '100%';
    //    i.style.top = '0';
    //    i.style.left = '0';
    //    var src = '{siteRoot}Tripmarklet?url=' + encodeURIComponent(document.URL) + '&title=' + encodeURIComponent(document.title);
    //    i.src = src;
    //    document.body.appendChild(i);

    //    var c = document.createElement('a');
    //    c.style.position = 'fixed';
    //    c.style.top = '10px';
    //    c.style.left = '10px';
    //    c.style.zIndex = 999999999;
    //    c.textContent = "closecloseclosecloseclose";
    //    c.setAttribute('href', "javascript: alert('bazinga'); var i = document.getElementById('triptitude_iframe'); i.parentNode.removeChild(i); this.parentNode.removeChild(this); ");
    //    document.body.appendChild(c);

})();