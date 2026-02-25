import { useState } from 'react';
import { Outlet } from 'react-router-dom';
import SidebarMenu from '@/components/layout/Sidebar/SidebarMenu';
import Header from '@/components/layout/Header/Header';

export default function Layout() {
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);

    const toggleSidebar = () => {
        setIsSidebarOpen(prev => !prev);
    };

    return (
        <div className="flex h-screen w-full bg-[#f8fafc] overflow-hidden">
            {/* Sidebar - fixed or transition width */}
            <SidebarMenu isOpen={isSidebarOpen} onToggle={toggleSidebar} />

            {/* Content Area */}
            <div className="flex-1 flex flex-col min-w-0 overflow-hidden relative">
                <Header />
                
                <main className="flex-1 overflow-y-auto overflow-x-hidden p-6 lg:p-10 scroll-smooth">
                <div className="max-w-7xl mx-auto">
                    <Outlet />
                </div>
                </main>
            </div>
        </div>
    )
}