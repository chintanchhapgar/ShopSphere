import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { useAuth } from "./useAuth";
import toast from "react-hot-toast";

const BACKEND_URL = import.meta.env.VITE_API_URL?.replace("/api", "") || "https://localhost:7065";

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: "info" | "success" | "warning" | "error";
  timestamp: string;
  read: boolean;
}

export const useSignalR = () => {
  const { token, isAuthenticated } = useAuth();
  const [connected, setConnected] = useState(false);
  const [notifications, setNotifications] = useState<Notification[]>(() => {
    const stored = localStorage.getItem("notifications");
    return stored ? JSON.parse(stored) : [];
  });
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!isAuthenticated || !token) return;

    // Create connection
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${BACKEND_URL}/hubs/notifications`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connectionRef.current = connection;

    // Handle incoming notifications
    connection.on("ReceiveNotification", (data: any) => {
      const notification: Notification = {
        id: `notif-${Date.now()}`,
        title: data.title,
        message: data.message,
        type: data.type || "info",
        timestamp: data.timestamp,
        read: false,
      };

      setNotifications((prev) => {
        const updated = [notification, ...prev].slice(0, 50);
        localStorage.setItem("notifications", JSON.stringify(updated));
        return updated;
      });

      // Show toast
      const toastOptions = { duration: 5000 };
      switch (notification.type) {
        case "success":
          toast.success(`${notification.title}\n${notification.message}`, toastOptions);
          break;
        case "error":
          toast.error(`${notification.title}\n${notification.message}`, toastOptions);
          break;
        default:
          toast(`${notification.title}\n${notification.message}`, toastOptions);
      }
    });

    // Start connection
    connection
      .start()
      .then(() => {
        console.log("✅ SignalR connected");
        setConnected(true);
      })
      .catch((err) => {
        console.error("❌ SignalR connection failed:", err);
        setConnected(false);
      });

    return () => {
      connection.stop();
      setConnected(false);
    };
  }, [isAuthenticated, token]);

  const markAsRead = (id: string) => {
    setNotifications((prev) => {
      const updated = prev.map((n) => (n.id === id ? { ...n, read: true } : n));
      localStorage.setItem("notifications", JSON.stringify(updated));
      return updated;
    });
  };

  const markAllAsRead = () => {
    setNotifications((prev) => {
      const updated = prev.map((n) => ({ ...n, read: true }));
      localStorage.setItem("notifications", JSON.stringify(updated));
      return updated;
    });
  };

  const clearAll = () => {
    setNotifications([]);
    localStorage.removeItem("notifications");
  };

  const unreadCount = notifications.filter((n) => !n.read).length;

  return {
    connected,
    notifications,
    unreadCount,
    markAsRead,
    markAllAsRead,
    clearAll,
  };
};