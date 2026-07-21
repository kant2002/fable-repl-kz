модуль WebComponent

// Web Components с Fable by Onur Gümüş (Twitter @OnurGumusDev)
// Check the custom tag ішінде the HTML tab and read this thread үшін more info:
// https://twitter.com/OnurGumusDev/status/1329019698667790337

// For a more high-level library to create Web Components, try Fable.Lit:
// https://fable.io/Fable.Lit/docs/web-components.html

ашық Fable.Core
ашық Browser
ашық Browser.Types
ашық Fable.Core.JsInterop

[<AllowNullLiteral>]
түрі HTMLTemplateElement =
    inherit HTMLElement
    abstract content: DocumentFragment с get, set

[<AllowNullLiteral>]
түрі HTMLTemplateElementType =
    [<EmitConstructor>]
    abstract Create: unit -> HTMLTemplateElement

болсын template: HTMLTemplateElement =
    downcast document.createElement ("template")

template.innerHTML <-
    """
  <style>
    .container {
      padding: 8px;
    }
    button {
      display: block;
      overflow: hidden;
      position: relative;
      padding: 0 16px;
      font-size: 16px;
      font-weight: bold;
      text-overflow: ellipsis;
      white-space: nowrap;
      cursor: pointer;
      outline: none;
      width: 100%;
      height: 40px;
      box-sizing: border-box;
      border: 1px solid #a1a1a1;
      background: #ffffff;
      box-shadow: 0 2px 4px 0 rgba(0,0,0, 0.05), 0 2px 8px 0 rgba(161,161,161, 0.4);
      color: #363636;
      cursor: pointer;
    }
  </style>
  <div class="container">
    <button>Label</button>
  </div>
"""

[<Global>]
модуль customElements =
    болсын define (elementName: string, ty: obj) = jsNative

[<Global>]
түрі ShadowRoot() =
    мүшесі this.appendChild(el: Browser.Types.Node) = jsNative
    мүшесі this.querySelector(selector: string): Browser.Types.HTMLElement = jsNative

болсын кіріктірілген attachStatic<'T> (name: string) (f: obj): unit = jsConstructor<'T>?name <- f

болсын кіріктірілген attachStaticGetter<'T, 'V> (name: string) (f: unit -> 'V): unit =
    JS.Constructors.Object.defineProperty (jsConstructor<'T>, name, !!{| get = f |})
    |> ignore

[<Global; AbstractClass>]
[<AllowNullLiteral>]
түрі HTMLElement() =
    мүшесі _.getAttribute(attr: string): string = jsNative
    мүшесі _.attachShadow(obj): ShadowRoot = jsNative
    abstract connectedCallback: unit -> unit
    abstract attributeChangedCallback: string * obj * obj -> unit

[<AllowNullLiteral>]
түрі Button() =
    inherit HTMLElement()

    болсын shadowRoot: ShadowRoot = base.attachShadow ({| mode = "ашық" |})

    жасау
        болсын clone = template.content.cloneNode (true)
        shadowRoot.appendChild (clone)

    болсын button = shadowRoot.querySelector ("button")

    мүшесі this.render() =
        button.innerHTML <- this.getAttribute ("label")

    override _.connectedCallback() = printf "connected callback"

    override this.attributeChangedCallback(name, oldVal, newVal) = this.render ()

attachStaticGetter<Button, _> "observedAttributes" (функ () -> [| "label" |])

customElements.define ("my-button", jsConstructor<Button>)
