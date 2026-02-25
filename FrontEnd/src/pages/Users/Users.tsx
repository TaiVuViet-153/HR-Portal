import React, { useEffect, useRef, useState } from 'react';
import { Plus, Edit2, Trash2, Eye, Lock, KeyRound, Key } from 'lucide-react';
import type { UserWithDetail, GetUsersQuery } from '@/pages/Users/UsersType';
import { getUsers, createUser, updateUser, deleteUser, resetPassword, lockUser } from './UserApi';
import { useAuth } from '@/core/auth/AuthContext';
import Loading from '@/components/common/Loading';
import SelectCommon from '@/components/common/SelectCommon';
import DatePickerCommon from '@/components/common/DatePickerCommon';
import UserDetailModal from './UserDetailModal';
import UserFormModal from './UserFormModal';
import UserDeleteModal from './UserDeleteModal';
import UserProfile from './UserProfile';
import ChangePasswordModal from './ChangePasswordModal';
import { SortByOptions, SortDirOptions, UserStatus, isLocked } from './UsersData';

export default function EmployeeList() {
  const { state } = useAuth();
  const isAdmin = state.user?.roles?.includes('ADMIN');

  const [users, setUsers] = useState<UserWithDetail[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const [modalType, setModalType] = useState<'create' | 'edit' | 'detail' | 'delete' | 'changePassword' | null>(null);
  const [selectedUser, setSelectedUser] = useState<UserWithDetail | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [totalPages, setTotalPages] = useState(0);

  const currentFilterRef = useRef<GetUsersQuery>({});

  const fetchData = async (query?: GetUsersQuery) => {
    try {
      setLoading(true);
      const data = await getUsers(query);
      if (data?.items) {
        setUsers(data.items);
        setCurrentPage(data.page ?? 1);
        setPageSize(data.pageSize ?? 10);
        setTotalItems(data.totalItems ?? 0);
        setTotalPages(data.totalPages ?? 0);
      } else {
        setUsers([]);
      }
    } catch (err) {
      setError("Fetch users failed");
      console.error("Fetch users failed", err);
      setUsers([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!state.accessToken || !isAdmin) return;
    fetchData({});
  }, [state.accessToken, isAdmin]);

  // If not admin, render UserProfile component
  if (!isAdmin) {
    return <UserProfile />;
  }

  const refetchWithCurrentParams = async (overrides?: Partial<GetUsersQuery>) => {
    const params: GetUsersQuery = {
      ...currentFilterRef.current,
      page: currentPage,
      pageSize: pageSize,
      ...overrides
    };
    await fetchData(params);
  };

  const handleFilter = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.target as HTMLFormElement);
    
    const toOptional = (key: string) => {
      const value = (formData.get(key) ?? '').toString().trim();
      return value !== '' ? value : undefined;
    };

    const search = toOptional('search');
    const createdAfter = toOptional('createdAfter');
    const createdBefore = toOptional('createdBefore');
    const status = toOptional('status');
    const sortBy = toOptional('sortBy');
    const sortDir = toOptional('sortDir');

    const queryData: GetUsersQuery = { 
      search, 
      createdAfter: createdAfter ? new Date(createdAfter) : undefined,
      createdBefore: createdBefore ? new Date(createdBefore) : undefined,
      status: status ? Number(status) : undefined,
      sortBy,
      sortDir: sortDir ? Number(sortDir) : undefined,
      page: 1, 
      pageSize 
    };
    currentFilterRef.current = queryData;
    await fetchData(queryData);
  };

  const handleResetFilter = async () => {
    currentFilterRef.current = {};
    await fetchData({ page: 1, pageSize });
  };

  const handlePageChange = async (newPage: number) => {
    if (newPage < 1 || newPage > totalPages) return;
    await refetchWithCurrentParams({ page: newPage });
  };

  const handlePageSizeChange = async (newPageSize: number) => {
    if (Number.isNaN(newPageSize)) return;
    await refetchWithCurrentParams({ page: 1, pageSize: newPageSize });
  };

  const handleClose = () => {
    setModalType(null);
    setSelectedUser(null);
    setError(null);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.target as HTMLFormElement);

    if (modalType === 'create') {
      const result = await createUser(formData);
      if (result?.isSuccess) {
        setSuccessMessage(`User created! Temporary password: ${result.data?.password}`);
        handleClose();
        await refetchWithCurrentParams();
      } else {
        setError(result?.message || "Create user failed");
      }
    } else if (modalType === 'edit' && selectedUser) {
      const result = await updateUser(selectedUser, formData);
      if (result?.isSuccess) {
        setSuccessMessage("User updated successfully!");
        handleClose();
        await refetchWithCurrentParams();
      } else {
        setError(result?.message || "Update user failed");
      }
    }
  };

  const handleDelete = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (selectedUser) {
      const result = await deleteUser(selectedUser.userID);
      if (result?.isSuccess) {
        setSuccessMessage("User deleted successfully!");
        handleClose();
        const newTotalItems = totalItems - 1;
        const newTotalPages = Math.max(1, Math.ceil(newTotalItems / pageSize));
        const targetPage = currentPage > newTotalPages ? newTotalPages : currentPage;
        await refetchWithCurrentParams({ page: targetPage });
      } else {
        setError(result?.message || "Delete user failed");
      }
    }
  };

  const handleResetPassword = async (userId: number) => {
    const result = await resetPassword(userId);
    if (result?.isSuccess) {
      setSuccessMessage(`Password reset! New password: ${result.data}`);
    } else {
      setError(result?.message || "Reset password failed");
    }
  };

  const handleLockUser = async (userId: number) => {
    const result = await lockUser(userId);
    if (result?.isSuccess) {
      setSuccessMessage("User locked successfully!");
      await refetchWithCurrentParams();
    } else {
      setError(result?.message || "Lock user failed");
    }
  };

  const getAvatarUrl = (user: UserWithDetail) => {
    if (user.detail?.UserPhoto) {
      return `data:image/jpeg;base64,${user.detail.UserPhoto}`;
    }
    return `https://api.dicebear.com/7.x/avataaars/svg?seed=${user.detail?.FullName || user.userID}`;
  };

  const formatDate = (dateString: string) => {
    const options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  const getStatusStyle = (status: number) => {
    switch (status) {
      case 1: return 'bg-green-50 text-green-600';
      case 0: return 'bg-gray-50 text-gray-600';
      case 2: return 'bg-amber-50 text-amber-600';
      default: return 'bg-blue-50 text-blue-600';
    }
  };

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      {/* Success Message Toast */}
      {successMessage && (
        <div className="fixed top-4 right-4 z-50 bg-green-500 text-white px-6 py-3 rounded-xl shadow-lg">
          <div className="flex items-center gap-3">
            <span>{successMessage}</span>
            <button onClick={() => setSuccessMessage(null)} className="text-white hover:text-green-100">×</button>
          </div>
        </div>
      )}

      {/* Error Message Toast */}
      {error && !modalType && (
        <div className="fixed top-4 right-4 z-50 bg-red-500 text-white px-6 py-3 rounded-xl shadow-lg">
          <div className="flex items-center gap-3">
            <span>{error}</span>
            <button onClick={() => setError(null)} className="text-white hover:text-red-100">×</button>
          </div>
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-6 bg-white p-8 rounded-3xl shadow-sm border border-gray-50">
        <div>
          <h1 className="text-3xl font-black text-gray-900 tracking-tight">Employees</h1>
          <p className="text-gray-500 font-medium">Directory of all team members and their roles.</p>
        </div>
        <div className="flex items-center gap-3">
          <button 
            onClick={() => setModalType('changePassword')}
            className="flex items-center gap-2 px-6 py-3 bg-indigo-600 text-white rounded-2xl font-bold hover:bg-indigo-700 transition-all shadow-lg shadow-indigo-100 cursor-pointer"
          >
            <Key size={20} />
            <span>Change Password</span>
          </button>
          <button 
            onClick={() => setModalType('create')}
            className="flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-2xl font-bold hover:bg-blue-700 transition-all shadow-lg shadow-blue-100 cursor-pointer"
          >
            <Plus size={20} />
            <span>Add Employee</span>
          </button>
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white p-6 rounded-3xl border border-gray-50 shadow-sm">
        <form onSubmit={handleFilter} className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-6 gap-4">
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Search</label>
            <input 
              name="search" 
              placeholder="Name or email..." 
              className="px-4 py-2 bg-gray-50 rounded-xl focus:ring-2 focus:ring-blue-500" 
            />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Created After</label>
            <DatePickerCommon name="createdAfter" disablePast={false} />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Created Before</label>
            <DatePickerCommon name="createdBefore" disablePast={false} />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Status</label>
            <SelectCommon
              name="status"
              options={UserStatus}
              isClearable
              placeholder="All"
            />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Sort By</label>
            <SelectCommon
              name="sortBy"
              options={SortByOptions}
              isClearable
              placeholder="Default"
            />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Sort Order</label>
            <SelectCommon
              name="sortDir"
              options={SortDirOptions}
              isClearable
              placeholder="Default"
            />
          </div>
          <div className="col-span-1 md:col-span-3 lg:col-span-6 flex justify-end gap-3">
            <button type="button" onClick={handleResetFilter} className="px-4 py-2 font-bold text-gray-600 bg-gray-100 rounded-xl hover:bg-gray-200 transition-all cursor-pointer">Reset</button>
            <button type="submit" className="px-4 py-2 font-bold bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition-all shadow-sm cursor-pointer">Apply Filters</button>
          </div>
        </form>
      </div>

      {/* User List - update action buttons */}
      {loading ? (
        <Loading />
      ) : (
        <div className="grid grid-cols-1 gap-4">
          {users.length === 0 ? (
            <div className="text-center py-20 text-gray-500 font-medium">No employees found.</div>
          ) : (
            users.map((user) => (
              <div key={user.userID} className="bg-white p-8 rounded-3xl border border-gray-50 shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all group">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-8">
                  <div className="flex items-start gap-5">
                    <div className="relative">
                      <img 
                        src={getAvatarUrl(user)} 
                        className="w-14 h-14 rounded-2xl shadow-sm border border-white object-cover" 
                        alt={user.firstName + ' ' + user.lastName || 'User'} 
                      />
                      <div className={`absolute -bottom-1 -right-1 w-4 h-4 rounded-full border-2 border-white ${
                        user.status === 1 ? 'bg-green-500' : user.status === 0 ? 'bg-gray-400' : 'bg-amber-500'
                      }`}></div>
                    </div>
                    <div>
                      <h3 className="font-black text-xl text-gray-900">
                        {user.firstName} {user.lastName}
                      </h3>
                      <div className="flex items-center gap-3 text-sm font-bold text-gray-400 mt-1">
                        <span className="text-gray-600">{user.email || 'No email'}</span>
                        <span className="h-1 w-1 bg-gray-200 rounded-full"></span>
                        <span>{user.location || 'No location'}</span>
                        <span className="h-1 w-1 bg-gray-200 rounded-full"></span>
                        <span className="text-cyan-600">Joined: {formatDate(user.createdAt)}</span>
                      </div>
                      <p className="text-gray-500 text-sm mt-4 font-medium italic border-l-4 border-gray-100 pl-4">
                        {user.phoneNumber || 'No phone number'}
                      </p>
                    </div>
                  </div>

                  <div className="flex items-center gap-6 border-t md:border-t-0 pt-6 md:pt-0">
                    <span className={`px-4 py-1.5 rounded-xl text-[10px] font-black uppercase tracking-widest shadow-sm ${getStatusStyle(user.status)}`}>
                      {UserStatus.find(status => status.value === user.status.toString())?.label || 'Unknown'}
                    </span>

                    <div className="h-10 w-[1px] bg-gray-100 hidden md:block"></div>

                    <div className="flex items-center gap-2">
                      <button 
                        onClick={() => { setSelectedUser(user); setModalType('detail'); }} 
                        className={`p-3 rounded-xl transition-all cursor-pointer text-sky-500 hover:bg-blue-50`}
                        title="View details"
                      >
                        <Eye size={20} />
                      </button>
                      <button 
                        onClick={() => { setSelectedUser(user); setModalType('edit'); }} 
                        className={`p-3 rounded-xl transition-all cursor-pointer text-slate-500 hover:bg-slate-50`}
                        title="Edit employee"
                      >
                        <Edit2 size={20} />
                      </button>
                      <button 
                        onClick={() => handleResetPassword(user.userID)} 
                        className={`p-3 rounded-xl transition-all text-amber-500 hover:bg-amber-50 cursor-pointer`}
                        title="Reset password"
                      >
                        <KeyRound size={20} />
                      </button>
                      <button 
                        onClick={() => handleLockUser(user.userID)} 
                        className={`p-3 rounded-xl transition-all ${isLocked(user.status) ? 'text-gray-400 cursor-not-allowed opacity-60' : 'text-orange-500 hover:bg-orange-50 cursor-pointer'}`}
                        title="Lock user"
                        disabled={isLocked(user.status)}
                      >
                        <Lock size={20} />
                      </button>
                      <button 
                        onClick={() => { setSelectedUser(user); setModalType('delete'); }} 
                        className="p-3 text-red-500 hover:bg-red-50 rounded-xl transition-all cursor-pointer"
                        title="Delete employee"
                      >
                        <Trash2 size={20} />
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
      )}

      {/* Pagination */}
      {totalItems > 0 && users.length !== 0 && (
        <div className="flex flex-col lg:flex-row items-start lg:items-center justify-between gap-4 bg-white p-4 rounded-2xl border border-gray-50 shadow-sm">
          <div className="flex items-center gap-3">
            <span className="text-sm font-medium text-gray-600">
              Page {currentPage} of {totalPages} · {totalItems} total
            </span>
            <SelectCommon
              name="pageSize"
              options={[5, 10, 20, 50].map(size => ({ value: String(size), label: `${size} / page` }))}
              defaultValue={pageSize}
              onChange={(val) => handlePageSizeChange(Number(val))}
            />
          </div>
          <div className="flex items-center gap-2">
            <button
              type="button"
              onClick={() => handlePageChange(currentPage - 1)}
              disabled={currentPage === 1 || loading}
              className="px-4 py-2 text-sm font-bold rounded-xl border border-gray-200 text-gray-600 disabled:opacity-50 hover:bg-gray-100 transition-all cursor-pointer disabled:cursor-not-allowed"
            >
              Previous
            </button>
            <button
              type="button"
              onClick={() => handlePageChange(currentPage + 1)}
              disabled={currentPage === totalPages || loading}
              className="px-4 py-2 text-sm font-bold rounded-xl border border-gray-200 text-gray-600 disabled:opacity-50 hover:bg-gray-100 transition-all cursor-pointer disabled:cursor-not-allowed"
            >
              Next
            </button>
          </div>
        </div>
      )}

      {/* MODALS */}
      <UserFormModal
        isOpen={modalType === 'create' || modalType === 'edit'}
        onClose={handleClose}
        onSubmit={handleSave}
        mode={modalType === 'edit' ? 'edit' : 'create'}
        user={selectedUser}
        error={error}
      />

      <UserDetailModal
        isOpen={modalType === 'detail'}
        onClose={handleClose}
        user={selectedUser}
      />

      <UserDeleteModal
        isOpen={modalType === 'delete'}
        onClose={handleClose}
        onConfirm={handleDelete}
        userName={selectedUser ? `${selectedUser.firstName} ${selectedUser.lastName}` : undefined}
      />

      <ChangePasswordModal
        isOpen={modalType === 'changePassword'}
        onClose={handleClose}
      />
    </div>
  );
}

