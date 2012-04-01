(function () {
    var overlayId = 'triptitude_overlay';

    var existing = document.getElementById(overlayId);
    if (existing) existing.parentNode.removeChild(existing);

    var overlay = document.createElement('div');
    overlay.setAttribute('id', overlayId);
    overlay.style.cssText = 'position: fixed; top: 0; right:0; bottom: 0; left: 0; width: 100%; background-color: rgba(242,242,242,0.7); z-index: 8675310; ';
    document.body.appendChild(overlay);

    var contentContainer = document.createElement('div');
    contentContainer.style.cssText = 'width: 960px; margin: 60px auto; height: 100%; position: relative;';
    overlay.appendChild(contentContainer);

    var c = document.createElement('a');
    c.style.cssText = 'position: absolute; right: 10px; top: 10px; z-index: 8675312;';
    c.textContent = "close";
    c.setAttribute('href', "javascript: var i = document.getElementById('" + overlayId + "'); i.parentNode.removeChild(i); this.parentNode.removeChild(this); ");
    contentContainer.appendChild(c);

    var i = document.createElement('iframe');
    i.style.cssText = "display: block; height: 100%; width: 100%; margin: 0; border: none; z-index: 8675311;";
    var src = '{siteRoot}Tripmarklet?url=' + encodeURIComponent(document.URL) + '&title=' + encodeURIComponent(document.title);
    i.src = src;
    contentContainer.appendChild(i);


})();