import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";
import Footer from "./Footer";
import AIChatbot from "@/components/features/AIChatbot";
import InstallPWA from "@/components/features/InstallPWA";
import OfflineIndicator from "@/components/features/OfflineIndicator";
import CompareBar from "@/components/features/CompareBar";

const Layout = () => {
  return (
    <div className="flex flex-col min-h-screen">
      <Navbar />
      <OfflineIndicator />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer />
      <CompareBar />
      <AIChatbot />
      <InstallPWA />
    </div>
  );
};

export default Layout;