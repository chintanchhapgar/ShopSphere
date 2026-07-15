import React from "react";
import ReactDOM from "react-dom/client";
import { Provider } from "react-redux";
import { BrowserRouter } from "react-router-dom";
import { Toaster } from "react-hot-toast";
import { store } from "@/redux/store";
import App from "@/App";
import "@/index.css";
import "@/i18n";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <Provider store={store}>
      <BrowserRouter>
        <App />
        <Toaster
          position="top-right"
          toastOptions={{
            duration: 3000,
            style: {
              background: "#fff",
              color: "#1f2937",
              boxShadow: "0 4px 6px -1px rgba(0,0,0,0.1)",
              borderRadius: "12px",
              padding: "12px 16px",
            },
            success: {
              iconTheme: { primary: "#2563eb", secondary: "#fff" },
            },
          }}
        />
      </BrowserRouter>
    </Provider>
  </React.StrictMode>
);