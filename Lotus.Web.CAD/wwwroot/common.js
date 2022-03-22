
// 
export function getBoundingClientRect(element)
{
    return element.getBoundingClientRect();
}

//
export function InitCadCanvas(cad_device, cad_canvas)
{
    window.cad =
    {
        device: cad_device,
        canvas: cad_canvas
    };

    if (window.cad.canvas)
    {
        window.cad.canvas.onmousemove = (e) =>
        {
            cad.device.invokeMethodAsync('OnMouseMove', e.clientX, e.clientY);
        };

        //window.cad.canvas.onmousedown = (e) =>
        //{
        //    cad.device.invokeMethodAsync('OnMouseDown', e.button, e.clientX, e.clientY);
        //};

        window.cad.canvas.onmouseup = (e) =>
        {
            cad.device.invokeMethodAsync('OnMouseUp', e.button, e.clientX, e.clientY);
        };

        window.cad.canvas.onkeydown = (e) =>
        {
            cad.device.invokeMethodAsync('OnKeyDown', e.keyCode);
        };

        window.cad.canvas.onkeyup = (e) =>
        {
            cad.device.invokeMethodAsync('OnKeyUp', e.keyCode);
        };

        //window.cad.canvas.onblur = (e) =>
        //{
        //    cad.canvas.canvas.focus();
        //};
        //window.cad.canvas.tabIndex = 0;
        //window.cad.canvas.focus();
    }

    window.addEventListener("resize", OnResizeCadCanvas);

    window.requestAnimationFrame(OnTickCadCanvas);
}

//
export function OnTickCadCanvas(time_stamp)
{
    //window.requestAnimationFrame(OnTickCadCanvas);
    //cad.device.invokeMethodAsync('OnTick', time_stamp);
}

//
export function OnResizeCadCanvas()
{
    if (!window.cad.canvas)
        return;

    var clien_rect = window.cad.canvas.getBoundingClientRect();

    cad.device.invokeMethodAsync('OnResize', clien_rect.width, clien_rect.height);
}