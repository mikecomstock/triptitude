(function () {
    console.log('Tripmarket loaded!');

    var i = document.createElement('iframe');
    i.style.position = 'fixed';
    i.style.zIndex = 9999999999;
    i.style.width = '100%';
    i.style.height = '100%';
    i.style.top = '0';
    i.style.left = '0';
    i.src = '{siteRoot}Tripmarklet?url=' + document.URL;
    document.body.appendChild(i);
})();