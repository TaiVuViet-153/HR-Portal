import { Navigate, Route, Routes } from 'react-router-dom'
import { RequireAuth } from '@/core/auth/RequireAuth'
import { RequireRole } from '@/core/auth/RequireRole'
import Layout from '@/components/layout/Layout'
import { Login } from '@/pages/Login/Login'
import LeaveRequests from '@/pages/Requests/LeaveRequests'
import Users from '@/pages/Users/Users'
import Forbidden from '@/pages/Error/Forbidden'
import NotFound from '@/pages/Error/NotFound'
import LeaveBalances from '@/pages/Balances/Balances'
import RequireChangePassword from '@/pages/Users/RequireChangePassword'

export default function AppRoutes() {
    return (
        <Routes>
            {/* Public route */}
            <Route path='/login' element={<Login />} />

            {/* Error pages */}
            <Route path='/403' element={<Forbidden />} />
            <Route path='/404' element={<NotFound />} />

            {/* Protected routes: RequireAuth -> MainLayout -> <Module /> */}
            <Route element={<RequireAuth />}>
                <Route path='/change-password' element={<RequireChangePassword />} />
                <Route element={<Layout />}>
                    <Route path='/' element={<Navigate to="/requests" replace/>} />
                    <Route path="/users" element={
                        // <RequireRole roles={['ADMIN']}>
                            <Users />
                        // </RequireRole>
                    } />
                    <Route path="/requests" element={                        
                        <RequireRole roles={['ADMIN', 'USER']}>
                            <LeaveRequests />
                        </RequireRole>
                    } />
                    <Route path="/balances" element={
                        <RequireRole roles={['ADMIN', 'USER']}>
                            <LeaveBalances />
                        </RequireRole>
                    } />
                </Route>
            </Route>

            {/* Fallback: 404 */}
            <Route path='*' element={<NotFound />} />
        </Routes>
    )
}
