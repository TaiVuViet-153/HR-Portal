import React, { useState, useEffect, useRef } from 'react';
import { Plus, Edit2, Trash2, Calendar } from 'lucide-react';
import type { GetBalancesQuery, GetBalancesResponse, LeaveBalance, SelectedBalance } from './BalancesType';
import { getBalances, createBalance, updateBalance, deleteBalance } from './BalancesApi';
import BalanceFormModal from './BalanceFormModal';
import BalanceDeleteModal from './BalanceDeleteModal';
import SelectCommon from '@/components/common/SelectCommon';
import DatePickerCommon from '@/components/common/DatePickerCommon';
import Loading from '@/components/common/Loading';
import { LeaveTypes, SortByOptions, SortDirOptions, getLeaveTypeKey, getLeaveTypeColor } from './BalancesData';
import { useAuth } from '@/core/auth/AuthContext';
import UserBalanceProfile from './UserBalanceProfile';



export default function LeaveBalances() {
  const { state } = useAuth();
  const isAdmin = state.user?.roles?.includes('ADMIN');

  const [balances, setBalances] = useState<GetBalancesResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const [modalType, setModalType] = useState<'create' | 'edit' | 'delete' | null>(null);
  const [selectedBalance, setSelectedBalance] = useState<SelectedBalance | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [totalPages, setTotalPages] = useState(0);

  const currentFilterRef = useRef<GetBalancesQuery>({});

  const fetchData = async (query?: GetBalancesQuery) => {
    try {
      setLoading(true);
      const data = await getBalances(query);
      if (data?.items) {
        setBalances(data.items);
        setCurrentPage(data.page ?? 1);
        setPageSize(data.pageSize ?? 10);
        setTotalItems(data.totalItems ?? 0);
        setTotalPages(data.totalPages ?? 0);
      } else {
        setBalances([]);
      }
    } catch (err) {
      setError("Fetch balances failed");
      console.error("Fetch balances failed", err);
      setBalances([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!state.accessToken || !isAdmin) return;
    fetchData({});
  }, [state.accessToken, isAdmin]);

  // If not admin, show user's own balance profile
  if (!isAdmin) {
    return <UserBalanceProfile />;
  }

  const refetchWithCurrentParams = async (overrides?: Partial<GetBalancesQuery>) => {
    const params: GetBalancesQuery = {
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
    const createdYear = toOptional('createdYear');
    const typeVal = toOptional('type');
    const sortBy = toOptional('sortBy');
    const sortDir = toOptional('sortDir');

    const queryData: GetBalancesQuery = { 
      search, 
      createdYear: createdYear ? Number(createdYear) : undefined,
      type: typeVal ? Number(typeVal) : undefined,
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
    setSelectedBalance(null);
    setError(null);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.target as HTMLFormElement);

    if (modalType === 'create') {
      const result = await createBalance(formData);
      if (result?.success) {
        setSuccessMessage(result?.message);
        handleClose();
        await refetchWithCurrentParams();
      } else {
        setError(result?.message || null);
      }
    } else if (modalType === 'edit' && selectedBalance) {
      const result = await updateBalance(selectedBalance, formData);
      if (result?.success) {
        setSuccessMessage(result?.message);
        handleClose();
        await refetchWithCurrentParams();
      } else {
        setError(result?.message || null);
      }
    }
  };

  const handleDelete = async () => {
    if (!selectedBalance) return;

    const { user, balanceItem } = selectedBalance;
    const result = await deleteBalance(user.userID, balanceItem.leaveType, balanceItem.year);

    if (result?.success) {
      setSuccessMessage(result?.message);
      handleClose();
      await refetchWithCurrentParams();
    } else {
      setError(result?.message || null);
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
          <h1 className="text-3xl font-black text-gray-900 tracking-tight">Leave Balances</h1>
          <p className="text-gray-500 font-medium">Manage leave balances for employees.</p>
        </div>
        <button 
          onClick={() => setModalType('create')}
          className="flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-2xl font-bold hover:bg-blue-700 transition-all shadow-lg shadow-blue-100 cursor-pointer"
        >
          <Plus size={20} />
          <span>Add Balance</span>
        </button>
      </div>

      {/* Filters */}
      <div className="bg-white p-6 rounded-3xl border border-gray-50 shadow-sm">
        <form onSubmit={handleFilter} className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-6 gap-4">
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Search</label>
            <input 
              name="search" 
              placeholder="Username, first name, last name or email..." 
              className="px-4 py-2 bg-gray-50 rounded-xl focus:ring-2 focus:ring-blue-500" 
            />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Year</label>
            <DatePickerCommon name="createdYear" views={["year"]} disablePast={false} />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Type</label>
            <SelectCommon
              name="type"
              options={LeaveTypes}
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

      {/* Balance List */}
      {loading ? (
        <Loading />
      ) : (
        <div className="grid grid-cols-1 gap-4">
          {balances.length === 0 ? (
            <div className="text-center py-20 text-gray-500 font-medium">No balances found.</div>
          ) : (
            balances.map((userBalance) => {
              // Group balances by year
              const balancesByYear = userBalance.leaveBalances?.reduce((acc, bal) => {
                const year = bal.year;
                if (!acc[year]) acc[year] = [];
                acc[year].push(bal);
                return acc;
              }, {} as Record<number, LeaveBalance[]>) || {};
              
              const years = Object.keys(balancesByYear).map(Number).sort((a, b) => b - a);

              return (
                <div key={userBalance.userID} className="bg-white p-6 rounded-3xl border border-gray-50 shadow-sm hover:shadow-md transition-all">
                  {/* User Info Header */}
                  <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 pb-4 border-b border-gray-100">
                    <div className="flex items-center gap-4">
                      <div className="w-12 h-12 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white font-bold text-lg">
                        {(userBalance.firstName?.[0] || userBalance.userName?.[0] || 'U').toUpperCase()}
                      </div>
                      <div>
                        <h3 className="font-black text-xl text-gray-900">
                          {(userBalance.firstName || userBalance.lastName) 
                            ? `${userBalance.firstName ?? ''} ${userBalance.lastName ?? ''}`.trim() 
                            : userBalance.userName}
                        </h3>
                        <p className="text-sm text-gray-500">{userBalance.email}</p>
                      </div>
                    </div>
                    <span className="text-xs font-medium text-gray-400">@{userBalance.userName}</span>
                  </div>

                  {/* Leave Balances by Year */}
                  {years.length === 0 ? (
                    <div className="py-6 text-center text-gray-400 text-sm">No leave balances</div>
                  ) : (
                    <div className="mt-4 space-y-4">
                      {years.map((year) => (
                        <div key={year}>
                          <div className="text-[10px] font-black text-gray-400 uppercase tracking-widest mb-3">
                            Year {year}
                          </div>
                          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-3">
                            {balancesByYear[year].map((bal, idx) => {
                              const colors = getLeaveTypeColor(bal.leaveType);
                              return (
                                <div
                                  key={`${year}-${bal.leaveType}-${idx}`}
                                  className={`flex items-center justify-between gap-3 p-3 rounded-xl ${colors.bg} group/item`}
                                >
                                  <div className="flex items-center gap-3">
                                    <div className={`w-10 h-10 rounded-lg ${colors.badge} flex items-center justify-center`}>
                                      <Calendar size={18} className={colors.text} />
                                    </div>
                                    <div>
                                      <span className={`text-[10px] font-black uppercase tracking-widest ${colors.text}`}>
                                        {getLeaveTypeKey(bal.leaveType)}
                                      </span>
                                      <p className={`text-lg font-bold ${colors.text}`}>
                                        {bal.balance} <span className="text-xs font-normal">days</span>
                                      </p>
                                    </div>
                                  </div>
                                  <div className="flex items-center gap-1 opacity-0 group-hover/item:opacity-100 transition-opacity">
                                    <button
                                      onClick={() => {
                                        setSelectedBalance({ user: userBalance, balanceItem: bal });
                                        setModalType('edit');
                                      }}
                                      className="p-1.5 hover:bg-white/50 text-blue-600 rounded-lg transition-all cursor-pointer"
                                      title="Edit balance"
                                    >
                                      <Edit2 size={14} />
                                    </button>
                                    <button
                                      onClick={() => {
                                        setSelectedBalance({ user: userBalance, balanceItem: bal });
                                        setModalType('delete');
                                      }}
                                      className="p-1.5 hover:bg-white/50 text-red-600 rounded-lg transition-all cursor-pointer"
                                      title="Delete balance"
                                    >
                                      <Trash2 size={14} />
                                    </button>
                                  </div>
                                </div>
                              );
                            })}
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              );
            })
          )}
        </div>
      )}

      {/* Pagination */}
      {totalItems > 0 && balances.length !== 0 && (
        <div className="flex flex-col lg:flex-row items-start lg:items-center justify-between gap-4 bg-white p-4 rounded-2xl border border-gray-50 shadow-sm">
          <div className="flex items-center gap-3">
            <span className="text-sm font-medium text-gray-600">
              Page {currentPage} of {totalPages} · {totalItems} total
            </span>
            <SelectCommon
              name="pageSize"
              options={[5, 10, 20, 50].map(size => ({ value: String(size), label: `${size} / page` }))}
              defaultValue={String(pageSize)}
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
      <BalanceFormModal
        isOpen={modalType === 'create' || modalType === 'edit'}
        onClose={handleClose}
        onSubmit={handleSave}
        mode={modalType === 'edit' ? 'edit' : 'create'}
        selectedData={selectedBalance}
        error={error}
      />

      <BalanceDeleteModal
        isOpen={modalType === 'delete'}
        onClose={handleClose}
        onConfirm={handleDelete}
        selectedBalance={selectedBalance}
        error={error}
      />
    </div>
  );
}
