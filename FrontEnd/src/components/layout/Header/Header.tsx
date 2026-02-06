import { Bell } from 'lucide-react';
import { useAuth } from "@/core/Auth/AuthContext";
import { useEffect, useState } from "react";


interface HeaderProps {
  onToggleMenu: () => void;
}

export default function Header({ onToggleMenu }: HeaderProps) {
    const { state } = useAuth();
    const [userName, setUserName] = useState(state.user?.name ?? "Guest");
    const [userId, setUserId] = useState(state.user?.id ?? "0");

    useEffect(() => {
        setUserName(state.user?.name ?? "Guest");
        setUserId(state.user?.id ?? "0");
    }, [state.user]);

    return (
        <header className="h-20 bg-white border-b border-gray-50 flex items-center justify-between px-6 lg:px-10 z-30 shrink-0">
            <div className="flex items-center gap-4">
                {/* <button 
                onClick={onToggleMenu} 
                className="invisible lg:inline-flex p-2.5 text-gray-500 hover:bg-gray-100 rounded-xl transition-colors active:scale-95"
                aria-label="Toggle Menu"
                >
                <Menu size={22} />
                </button> */}
                <h2 className="hidden sm:block text-lg font-bold text-gray-800 tracking-tight">Management Console</h2>
            </div>

            <div className="flex items-center gap-4 lg:gap-6">
                <button className="p-2.5 text-gray-400 hover:bg-gray-50 rounded-xl relative transition-all">
                <Bell size={20} />
                <span className="absolute top-2.5 right-2.5 w-2.5 h-2.5 bg-red-500 rounded-full border-2 border-white"></span>
                </button>
                <div className="h-8 w-[1px] bg-gray-100"></div>
                <div className="flex items-center gap-3">
                <div className="text-right hidden md:block">
                    <p className="text-sm font-bold text-gray-900 leading-tight">{userName}</p>
                    <p className="text-[10px] uppercase tracking-wider font-black text-blue-600 bg-blue-50 px-2 rounded-md inline-block">
                    {state.user?.roles}
                    </p>
                </div>
                <img 
                    src={`https://api.dicebear.com/7.x/avataaars/svg?seed=${userId}`} 
                    className="w-11 h-11 rounded-2xl object-cover border-2 border-white shadow-sm" 
                    alt="User profile" 
                />
                </div>
            </div>
        </header>
    );
}
