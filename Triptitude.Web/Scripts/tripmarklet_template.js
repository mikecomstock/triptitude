(function () {
    console.log('Tripmarket loaded!');

    var id = 'triptitude_iframe';
    var existing = document.getElementById(id);
    if (existing)
        existing.parentNode.removeChild(existing);

    var i = document.createElement('iframe');
    i.setAttribute('id', id);
    i.style.position = 'fixed';
    i.style.zIndex = 9999999999;
    i.style.width = '100%';
    i.style.height = '100%';
    i.style.top = '0';
    i.style.left = '0';
    i.src = '{siteRoot}Tripmarklet?url=' + encodeURIComponent(document.URL) + '&title=' + encodeURIComponent(document.title);
    document.body.appendChild(i);
})();