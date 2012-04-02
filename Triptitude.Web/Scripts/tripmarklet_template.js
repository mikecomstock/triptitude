(function () {
    var overlayId = 'triptitude_overlay';

    var existing = document.getElementById(overlayId);
    if (existing) existing.parentNode.removeChild(existing);

    var close = function () {
        overlay.parentNode.removeChild(overlay);
        contentContainer.parentNode.removeChild(contentContainer);
    };

    var overlay = document.createElement('div');
    overlay.setAttribute('id', overlayId);
    overlay.style.cssText = 'position: fixed; top: 0; right:0; bottom: 0; left: 0; width: 100%; background-color: rgba(242,242,242,0.7); z-index: 8675310;';
    overlay.onclick = close;
    document.body.appendChild(overlay);

    var contentContainer = document.createElement('div');
    contentContainer.style.cssText = 'position: fixed; top: 0; left: 0; bottom: 0; right: 0; width: 960px; margin: 60px auto 0 auto; z-index: 8675311;';
    document.body.appendChild(contentContainer);

    var c = document.createElement('div');
    c.style.cssText = 'position: absolute; right: -8px; top: -8px; z-index: 8675312; cursor: pointer; font-family: Tahoma,Arial,sans-serif; font-size: 22px; height: 22px; width: 22px; line-height: 21px; text-align: center; font-weight: bold; font-style: normal; color: #FFF; background-color: #A00; border: 1px solid #700; box-shadow: 1px 1px 5px #888; border-radius: 50px; display: block;';
    c.textContent = '×';
    c.setAttribute('title', 'Close');
    c.onclick = close;
    contentContainer.appendChild(c);

    var i = document.createElement('iframe');
    i.style.cssText = "display: block; height: 100%; width: 100%; margin: 0; border: none; z-index: 8675311;";
    var src = '{siteRoot}/tripmarklet/tripmarklet?url=' + encodeURIComponent(document.URL) + '&title=' + encodeURIComponent(document.title);
    i.src = src;
    contentContainer.appendChild(i);
})();