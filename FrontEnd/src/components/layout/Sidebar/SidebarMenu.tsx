import { useAuth } from '@/core/auth/AuthContext';
import { LogOut } from 'lucide-react';
import { SidebarData, type SidebarMenuItem } from '@/components/layout/Sidebar/SidebarData';
import SidebarItem from '@/components/layout/Sidebar/SidebarItem';
import { logoutApi } from '@/pages/Login/LoginApi';



interface SideMenuProps {
  isOpen: boolean;
  onToggle?: () => void;
}



export default function SidebarMenu({ isOpen, onToggle }: SideMenuProps) {
    const { logout } = useAuth();

    const handleLogout = async () => {
      await logoutApi();
      logout();
    }

    return (
    <aside className={`${isOpen ? 'w-72' : 'w-20'} bg-white border-r border-gray-100 transition-all duration-300 flex flex-col p-4 z-40 relative`}>
      {/* Mobile hamburger: visible on small viewports, positioned at top-left of sidebar */}
      {onToggle && (
        <div className="lg:hidden absolute top-4 left-4 z-50">
          <button
            onClick={onToggle}
            aria-label={isOpen ? 'Close menu' : 'Open menu'}
            aria-expanded={isOpen}
            className="p-2.5 bg-white text-gray-600 hover:bg-gray-50 rounded-xl shadow-sm transition-transform active:scale-95"
          >
            {/* simple hamburger icon lines */}
            <div className="w-5 h-5 relative">
              <span className={`block absolute left-0 right-0 h-[2px] bg-current transition-transform ${isOpen ? 'translate-y-2 rotate-45' : '-translate-y-1'}`} />
              <span className={`block absolute left-0 right-0 h-[2px] bg-current transition-opacity ${isOpen ? 'opacity-0' : 'opacity-100'}`} />
              <span className={`block absolute left-0 right-0 h-[2px] bg-current transition-transform ${isOpen ? 'translate-y-2 -rotate-45' : 'translate-y-2'}`} />
            </div>
          </button>
        </div>
      )}
            <div className="py-6 flex items-center gap-3 mb-6">
                <div className="w-10 h-10 bg-blue-600 rounded-xl flex items-center justify-center text-white font-black text-xl shadow-lg shadow-blue-100 shrink-0">L</div>
                {isOpen && <span className="text-xl font-black text-gray-900 tracking-tight whitespace-nowrap">Linh Long HRM</span>}
            </div>
            <nav className="flex-1">
                {
                  SidebarData.map((item: SidebarMenuItem, index: number) => {
                    return (
                      <SidebarItem 
                        key={index}
                        to={item.to}
                        icon={item.icon}
                        label={item.label}
                        roles={item.roles}
                        collapsed={!isOpen}
                      />
                    )
                  })
                }
            </nav>

            <div className="pt-4 border-t border-gray-50">
              <button 
                onClick={handleLogout}
                className="flex items-center gap-3 px-4 py-3 w-full text-gray-500 hover:bg-red-50 hover:text-red-600 rounded-xl transition-colors font-semibold overflow-hidden cursor-pointer"
              >
                <LogOut size={20} className="shrink-0" />
                {isOpen && <span className="whitespace-nowrap">Logout</span>}
              </button>
            </div>
        </aside>
    )
}

