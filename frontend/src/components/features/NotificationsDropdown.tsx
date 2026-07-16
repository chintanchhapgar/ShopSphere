import { useState } from "react";
import { Bell, X, CheckCheck, Trash2, Info, CheckCircle, AlertTriangle, XCircle } from "lucide-react";
import { useSignalR } from "@/hooks/useSignalR";
import { cn } from "@/utils/cn";
import { useAuth } from "@/hooks/useAuth";
import { useClickOutside } from "@/hooks/useClickOutside";

const iconMap = {
  info:    { icon: Info,           color: "text-blue-500 bg-blue-50" },
  success: { icon: CheckCircle,    color: "text-green-500 bg-green-50" },
  warning: { icon: AlertTriangle,  color: "text-yellow-500 bg-yellow-50" },
  error:   { icon: XCircle,        color: "text-red-500 bg-red-50" },
};

const NotificationsDropdown = () => {
  const { isAuthenticated } = useAuth();
  const { notifications, unreadCount, connected, markAsRead, markAllAsRead, clearAll } = useSignalR();
  const [isOpen, setIsOpen] = useState(false);

  const ref = useClickOutside<HTMLDivElement>(
    () => setIsOpen(false),
    isOpen
  );

  if (!isAuthenticated) return null;

  const timeAgo = (timestamp: string) => {
    const seconds = Math.floor((Date.now() - new Date(timestamp).getTime()) / 1000);
    if (seconds < 60) return "just now";
    if (seconds < 3600) return `${Math.floor(seconds / 60)}m ago`;
    if (seconds < 86400) return `${Math.floor(seconds / 3600)}h ago`;
    return `${Math.floor(seconds / 86400)}d ago`;
  };

  return (
    <div ref={ref} className="relative">
      {/* Bell Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="relative p-2 text-gray-600 hover:text-primary-600 hover:bg-primary-50 dark:text-gray-300 dark:hover:bg-gray-800 rounded-lg transition"
        title="Notifications"
      >
        <Bell className="w-5 h-5" />
        {unreadCount > 0 && (
          <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center font-bold animate-pulse">
            {unreadCount > 9 ? "9+" : unreadCount}
          </span>
        )}
        {connected && (
          <span className="absolute bottom-1 right-1 w-2 h-2 bg-green-500 rounded-full" />
        )}
      </button>

      {/* Dropdown */}
      {isOpen && (
        <>
          <div className="fixed inset-0 z-10" onClick={() => setIsOpen(false)} />
          <div className="absolute right-0 top-full mt-2 w-96 bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-lg z-20 max-h-[500px] flex flex-col">

            {/* Header */}
            <div className="flex items-center justify-between p-4 border-b border-gray-100 dark:border-gray-700">
              <div>
                <h3 className="font-bold text-gray-900 dark:text-gray-100">Notifications</h3>
                <p className="text-xs text-gray-500 dark:text-gray-400 flex items-center gap-1">
                  <span className={cn("w-2 h-2 rounded-full", connected ? "bg-green-500" : "bg-red-500")} />
                  {connected ? "Live" : "Disconnected"}
                </p>
              </div>
              {notifications.length > 0 && (
                <div className="flex gap-1">
                  <button
                    onClick={markAllAsRead}
                    className="p-1.5 text-gray-400 hover:text-primary-600 rounded transition"
                    title="Mark all as read"
                  >
                    <CheckCheck className="w-4 h-4" />
                  </button>
                  <button
                    onClick={clearAll}
                    className="p-1.5 text-gray-400 hover:text-red-600 rounded transition"
                    title="Clear all"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              )}
            </div>

            {/* List */}
            <div className="flex-1 overflow-y-auto">
              {notifications.length === 0 ? (
                <div className="p-8 text-center">
                  <Bell className="w-12 h-12 text-gray-200 dark:text-gray-700 mx-auto mb-3" />
                  <p className="text-gray-500 dark:text-gray-400 text-sm">No notifications yet</p>
                  <p className="text-xs text-gray-400 dark:text-gray-500 mt-1">
                    You'll be notified about orders and updates
                  </p>
                </div>
              ) : (
                <div className="divide-y divide-gray-50 dark:divide-gray-700">
                  {notifications.map((notif) => {
                    const { icon: Icon, color } = iconMap[notif.type] || iconMap.info;
                    return (
                      <div
                        key={notif.id}
                        onClick={() => markAsRead(notif.id)}
                        className={cn(
                          "p-4 hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer transition",
                          !notif.read && "bg-primary-50/50 dark:bg-primary-900/20"
                        )}
                      >
                        <div className="flex gap-3">
                          <div className={cn("p-2 rounded-lg shrink-0 h-fit", color)}>
                            <Icon className="w-4 h-4" />
                          </div>
                          <div className="flex-1 min-w-0">
                            <div className="flex items-start justify-between gap-2">
                              <p className={cn("text-sm font-semibold", !notif.read ? "text-gray-900 dark:text-gray-100" : "text-gray-700 dark:text-gray-300")}>
                                {notif.title}
                              </p>
                              {!notif.read && (
                                <span className="w-2 h-2 bg-primary-600 rounded-full shrink-0 mt-1.5" />
                              )}
                            </div>
                            <p className="text-xs text-gray-600 dark:text-gray-400 mt-1">{notif.message}</p>
                            <p className="text-xs text-gray-400 dark:text-gray-500 mt-1">{timeAgo(notif.timestamp)}</p>
                          </div>
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default NotificationsDropdown;