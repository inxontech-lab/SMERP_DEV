# SMERPWeb mixed render-mode setup (Server + WebAssembly)

This project is a **Blazor Web App** with both interactive hosting models enabled in `Program.cs`:

- `AddInteractiveServerComponents()`
- `AddInteractiveWebAssemblyComponents()`
- `AddInteractiveServerRenderMode()`
- `AddInteractiveWebAssemblyRenderMode()`

The final part needed is to keep the app root in `InteractiveAuto` and then choose render mode **per page/component**.

## Step-by-step

1. **Keep app shell in Auto mode**
   - In `Components/App.razor`, use:
     - `<HeadOutlet @rendermode="InteractiveAuto" />`
     - `<Routes @rendermode="InteractiveAuto" />`

2. **Choose Server mode for a page**
   - In the page/component file add:

   ```razor
   @rendermode InteractiveServer
   @page "/tenant"
   ```

   Use this for pages that need server-only resources or long-lived server circuits.

3. **Choose WebAssembly mode for a page**
   - In the page/component file add:

   ```razor
   @rendermode InteractiveWebAssembly
   @page "/supplier"
   ```

4. **Place WASM pages where the client can load them**
   - Any page that runs as `InteractiveWebAssembly` should live in `SMERPWeb.Client` (or another shared project referenced by `SMERPWeb.Client`).
   - If a page stays in the server project only, it may not be available to the WebAssembly runtime.

5. **Register client-side services for WASM pages**
   - Dependencies used by WASM pages must be registered in `SMERPWeb.Client/Program.cs`.
   - Server-only DI registrations in `SMERPWeb/Program.cs` are not automatically available in browser runtime.

6. **Keep SSR-safe lifecycle usage**
   - Avoid JS interop in `OnInitialized{Async}` for components that prerender.
   - Use `OnAfterRenderAsync(firstRender)` when browser APIs are required.

7. **Verify route behavior**
   - Navigate to one server-mode page and one wasm-mode page.
   - Confirm both work and that wasm-mode page dependencies are resolved in client DI.

## Suggested migration pattern for current SMERPWeb forms

- Start with pages that mostly call HTTP APIs and use browser-side interactions (good WASM candidates), e.g. inventory list/forms.
- Keep authentication-sensitive or server-resource-heavy pages in `InteractiveServer` initially.
- Move shared UI into a shared class library if you want to reuse the same components in both hosting models with minimal duplication.
