import React, { useEffect, useRef, useState } from 'react';
import { Plus, Calendar, Check, X, Eye, Edit2, Trash2, Ban } from 'lucide-react';
import { type GetRequestQuery, type LeaveRequestResponse } from '@/pages/Requests/LeaveRequestsType';
import Loading from '@/components/common/Loading';
import DatePickerCommon from '@/components/common/DatePickerCommon';
import SelectCommon from '@/components/common/SelectCommon';
import { useAuth } from '@/core/auth/AuthContext';
import { createRequest, deleteRequest, getRequest, updateRequest } from './RequestApi';
import { RequestStatuses, RequestTypes, isPending, isApproved, getStatusKey } from './LeaveRequestsData';
import LeaveRequestDetailModal from './LeaveRequestDetailModal';
import LeaveRequestFormModal from './LeaveRequestFormModal';
import LeaveRequestDeleteModal from './LeaveRequestDeleteModal';
import LeaveRequestCancelModal from './LeaveRequestCancelModal';

export default function LeaveRequests() {
  const { state } = useAuth();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [requests, setRequests] = useState<LeaveRequestResponse[] | null>(null);
  const [modalType, setModalType] = useState<'create' | 'edit' | 'delete' | 'cancel' | 'detail' | null>(null);
  const [selectedRequest, setSelectedRequest] = useState<LeaveRequestResponse | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [totalItems, setTotalItems] = useState(0);
  const [totalPages, setTotalPages] = useState(0);

  // Store current filter params for reuse
  const currentFilterRef = useRef<GetRequestQuery>({} as GetRequestQuery);

  const getBaseQuery = (): GetRequestQuery => {
    const params: GetRequestQuery = {} as GetRequestQuery;
    if (state.user?.roles && !state.user.roles.includes('ADMIN')) {
      params.userId = Number(state.user?.id);
    }
    return params;
  };

  useEffect(() => {
    if (!state.accessToken) return;
    const params = getBaseQuery();
    currentFilterRef.current = params;
    fetchData(params);
  }, [state.accessToken]);

  const fetchData = async (queryData?: GetRequestQuery) => {
    try {
      setLoading(true);
      const data = await getRequest(queryData);
      setRequests(Array.isArray(data?.items) ? data.items : []);
      setCurrentPage(data?.page ?? 1);
      setPageSize(data?.pageSize ?? 5);
      setTotalItems(data?.totalItems ?? 0);
      setTotalPages(data?.totalPages ?? 0);
    } catch (err) {
      setError("Fetch requests failed");
      console.error("Fetch requests failed", err);
      setRequests([]);
    } finally {
      setLoading(false);
    }
  };

  const refetchWithCurrentParams = async (overrides?: Partial<GetRequestQuery>) => {
    const params: GetRequestQuery = {
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

    const queryData: GetRequestQuery = getBaseQuery();

    const type = toOptional('type');
    const startDate = toOptional('startDate');
    const endDate = toOptional('endDate');
    const isHalfDayOff = toOptional('isHalfDayOff');
    const reason = toOptional('reason');
    const status = toOptional('status');

    if (type) queryData.type = Number(type);
    if (startDate) queryData.startDate = startDate;
    if (endDate) queryData.endDate = endDate;
    if (isHalfDayOff) queryData.isHalfDayOff = Number(isHalfDayOff) === 1;
    if (reason) queryData.reason = reason;
    if (status) queryData.status = Number(status);

    // Reset to page 1 when filtering
    queryData.page = 1;
    queryData.pageSize = pageSize;

    currentFilterRef.current = queryData;
    await fetchData(queryData);
  };

  const handleResetFilter = async () => {
    const params = getBaseQuery();
    params.page = 1;
    params.pageSize = pageSize;
    currentFilterRef.current = params;
    await fetchData(params);
  };

  const handlePageChange = async (newPage: number) => {
    if (newPage < 1 || newPage > totalPages) return;
    await refetchWithCurrentParams({ page: newPage });
  };

  const handlePageSizeChange = async (newPageSize: number) => {
    if (Number.isNaN(newPageSize)) return;
    // Reset to page 1 when changing page size
    await refetchWithCurrentParams({ page: 1, pageSize: newPageSize });
  };

  const handleClose = () => {
    setModalType(null);
    setError(null);
    setSelectedRequest(null);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData(e.target as HTMLFormElement);

    if (modalType === 'create') {
      const createdRequest = await createRequest(Number(state.user?.id), formData);
      if (createdRequest !== undefined) {
        handleClose();
        await refetchWithCurrentParams();
      } else {
        setError("Create request failed");
        console.error("Create request failed");
      }
    } else if (modalType === 'edit' && selectedRequest) {
      const updatedRequest = await updateRequest(Number(state.user?.id), selectedRequest, formData);
      if (updatedRequest !== undefined) {
        handleClose();
        setRequests(prev => (prev ?? []).map(r => r.requestId === updatedRequest.requestId ? updatedRequest : r));
      } else {
        setError("Update request failed");
        console.error("Update request failed");
      }
    }
  };

  const handleCancel = async (e: React.FormEvent) => {
    e.preventDefault();
    let handleSuccess = false;

    if (selectedRequest) {
      const updatingRequest = { ...selectedRequest, status: 'Cancelled' };
      const updatedRequest = await updateRequest(Number(state.user?.id), updatingRequest, undefined);
      if (updatedRequest !== undefined) {
        setRequests(prev => (prev ?? []).map(r => r.requestId === updatedRequest.requestId ? updatedRequest : r));
        handleSuccess = true;
      }
    }

    if (handleSuccess) {
      handleClose();
      // await refetchWithCurrentParams();
    }
  };

  const handleDelete = async (e: React.FormEvent) => {
    e.preventDefault();
    let handleSuccess = false;

    if (selectedRequest) {
      const deletedRequest = await deleteRequest(selectedRequest.requestId);
      if (deletedRequest) {
        handleSuccess = true;
      } else {
        setError("Delete request failed");
        console.error("Delete request failed");
      }
    }

    if (handleSuccess) {
      handleClose();
      // After delete, check if current page is still valid
      const newTotalItems = totalItems - 1;
      const newTotalPages = Math.max(1, Math.ceil(newTotalItems / pageSize));
      const targetPage = currentPage > newTotalPages ? newTotalPages : currentPage;
      await refetchWithCurrentParams({ page: targetPage });
    }
  };

  const updateStatus = async (request: LeaveRequestResponse, status: number) => {
    const updatingRequest = { ...request, status: getStatusKey(status) };
    const updatedRequest = await updateRequest(Number(state.user?.id), updatingRequest, undefined);

    if (updatedRequest !== undefined) {
      // await refetchWithCurrentParams();
      setRequests(prev => (prev ?? []).map(r => r.requestId === updatedRequest.requestId ? updatedRequest : r));
    } else {
      setError("Update status failed");
      console.error("Update status failed");
    }
  };

  const formatDate = (dateString: string) => {
    const options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  const formatDateTime = (dateString: string) => {
    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric', month: 'short', day: 'numeric',
      hour: '2-digit', minute: '2-digit', second: '2-digit'
    };
    return new Date(dateString).toLocaleString(undefined, options);
  };

  const calculateDays = (request: LeaveRequestResponse) => {
    const start = new Date(request.startDate);
    const end = new Date(request.endDate);
    const diffTime = Math.abs(end.getTime() - start.getTime());
    const diffDays = diffTime === 0
      ? (request.isHalfDayOff ? 0.5 : 1)
      : Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1 + (request.isHalfDayOff ? 0.5 : 0);
    
    return diffDays;
  };

  return (
    <div className="space-y-8 animate-in fade-in duration-500">
      <div className="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-6 bg-white p-8 rounded-3xl shadow-sm border border-gray-50">
        <div>
          <h1 className="text-3xl font-black text-gray-900 tracking-tight">Leave Management</h1>
          <p className="text-gray-500 font-medium">Monitor and approve employee time-off requests.</p>
        </div>
        <button 
          onClick={() => setModalType('create')}
          className="flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-2xl font-bold hover:bg-blue-700 transition-all shadow-lg shadow-blue-100 cursor-pointer"
        >
          <Plus size={20} />
          <span>New Request</span>
        </button>
      </div>

      <div className="bg-white p-6 rounded-3xl border border-gray-50 shadow-sm">
        <form onSubmit={handleFilter} className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-6 gap-4">
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Type</label>
            <SelectCommon
              name="type"
              options={Object.entries(RequestTypes).map(([key, value]) => ({ value: String(value), label: key }))}
              isClearable
              placeholder="All"
            />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Start Date</label>
            <DatePickerCommon name="startDate" disablePast={false} />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">End Date</label>
            <DatePickerCommon name="endDate" disablePast={false} />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Half Day</label>
            <SelectCommon
              name="isHalfDayOff"
              options={[
                { value: '1', label: 'Yes' },
                { value: '0', label: 'No' }
              ]}
              isClearable
              placeholder="Any"
            />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Reason</label>
            <input name="reason" placeholder="Search reason" className="px-3 py-2 bg-gray-50 rounded-xl focus:ring-2 focus:ring-blue-500" />
          </div>
          <div className="flex flex-col">
            <label className="text-[11px] font-black text-gray-400 uppercase tracking-widest mb-2">Status</label>
            <SelectCommon
              name="status"
              options={Object.entries(RequestStatuses).map(([key, value]) => ({ value: String(value), label: key }))}
              isClearable
              placeholder="All"
            />
          </div>
          <div className="col-span-1 md:col-span-3 lg:col-span-6 flex justify-end gap-3">
            <button type="button" onClick={handleResetFilter} className="px-4 py-2 font-bold text-gray-600 bg-gray-100 rounded-xl hover:bg-gray-200 transition-all cursor-pointer">Reset</button>
            <button type="submit" className="px-4 py-2 font-bold bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition-all shadow-sm cursor-pointer">Apply Filters</button>
          </div>
        </form>
      </div>

      {loading ? (
        <Loading />
      ) :
        <div className="grid grid-cols-1 gap-4">
          {
            requests?.length === 0 
            ? (
              <div className="text-center py-20 text-gray-500 font-medium">No leave requests found.</div>
            )
            :
            requests?.map((request) => (
              <div key={request.requestId} className="bg-white p-8 rounded-3xl border border-gray-50 shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all group">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-8">
                  <div className="flex items-start gap-5">
                    <div className={`w-14 h-14 rounded-2xl flex items-center justify-center ${
                      request.type === 'Paid' ? 'bg-blue-50 text-blue-600' :
                      request.type === 'Unpaid' ? 'bg-red-50 text-red-600' : 'bg-purple-50 text-purple-600'
                    }`}>
                      <Calendar size={28} />
                    </div>
                    <div>
                      <h3 className="font-black text-xl text-gray-900">{request.userName}</h3>
                      <div className="flex items-center gap-3 text-sm font-bold text-gray-400 mt-1">
                        <span className="text-gray-600 uppercase tracking-widest text-[10px]">{request.type} Leave</span>
                        <span className="h-1 w-1 bg-gray-200 rounded-full"></span>
                        <span>{formatDate(request.startDate)} — {formatDate(request.endDate)}</span>
                        <span className="h-1 w-1 bg-gray-200 rounded-full"></span>
                        <span>{calculateDays(request)} days</span>
                        <span className="h-1 w-1 bg-gray-200 rounded-full"></span>
                        <span className='text-cyan-600'>Submitted: {formatDateTime(request.createdAt)}</span>
                      </div>
                      <p className="text-gray-500 text-sm mt-4 font-medium italic border-l-4 border-gray-100 pl-4">"{request.reason}"</p>
                    </div>
                  </div>

                  <div className="flex items-center gap-6 border-t md:border-t-0 pt-6 md:pt-0">
                    <span className={`px-4 py-1.5 rounded-xl text-[10px] font-black uppercase tracking-widest shadow-sm ${
                      isPending(request.status) ? 'bg-yellow-50 text-yellow-600' :
                      isApproved(request.status) ? 'bg-green-50 text-green-600' :
                      request.status === 'Rejected' ? 'bg-red-50 text-red-600' : 'bg-blue-50 text-blue-600'
                    }`}>
                      {request.status}
                    </span>

                    <div className="h-10 w-[1px] bg-gray-100 hidden md:block"></div>

                    <div className="flex items-center gap-2">
                      {isPending(request.status) && state.user?.roles?.includes('ADMIN') && (
                        <>
                          <button onClick={() => updateStatus(request, RequestStatuses.Approved)} className="p-3 bg-green-50 text-green-600 rounded-xl hover:bg-green-600 hover:text-white transition-all shadow-sm cursor-pointer"><Check size={20} /></button>
                          <button onClick={() => updateStatus(request, RequestStatuses.Rejected)} className="p-3 bg-red-50 text-red-600 rounded-xl hover:bg-red-600 hover:text-white transition-all shadow-sm cursor-pointer"><X size={20} /></button>
                        </>
                      )}
                      <button onClick={() => { setSelectedRequest(request); setModalType('detail'); }} className="p-3 text-gray-400 hover:bg-gray-50 rounded-xl transition-all cursor-pointer"><Eye size={20} /></button>
                      <button 
                        onClick={() => { setSelectedRequest(request); setModalType('edit'); }} 
                        className={`p-3 text-gray-400 hover:bg-gray-50 rounded-xl transition-all ${!isPending(request.status) ? 'cursor-not-allowed opacity-60' : 'cursor-pointer'}`}
                        disabled={!isPending(request.status)}
                        title={!isPending(request.status) ? 'Only Edit pending request' : undefined}
                      >
                        <Edit2 size={20} />
                      </button>
                      <button 
                        onClick={() => { setSelectedRequest(request); setModalType('cancel'); }} 
                        className={`p-3 text-gray-400 hover:bg-gray-50 rounded-xl transition-all ${!isPending(request.status) ? 'cursor-not-allowed opacity-60' : 'cursor-pointer'}`}
                        disabled={!isPending(request.status)}
                        title={!isPending(request.status) ? 'Only Cancel pending request' : undefined}
                      >
                        <Ban size={20} />
                      </button>
                      {state.user?.roles?.includes('ADMIN') && (
                        <button 
                          onClick={() => { setSelectedRequest(request); setModalType('delete'); }} 
                          className={`p-3 text-red-500 hover:bg-red-50 rounded-xl transition-all ${isApproved(request.status) ? 'cursor-not-allowed opacity-60' : 'cursor-pointer'}`}
                          disabled={isApproved(request.status)}
                          title={isApproved(request.status) ? 'Admin only delete pending request' : undefined}
                        >
                          <Trash2 size={20} />
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))
          }
          
        </div>
      }

      {/* Pagination - updated handlers */}
      {totalItems > 0 && requests?.length !== 0 && (
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
      <LeaveRequestFormModal
        isOpen={modalType === 'create' || modalType === 'edit'}
        onClose={handleClose}
        onSubmit={handleSave}
        mode={modalType === 'edit' ? 'edit' : 'create'}
        request={selectedRequest}
        userName={state.user?.name}
        error={error}
      />

      <LeaveRequestDetailModal 
        isOpen={modalType === 'detail'}
        onClose={handleClose}
        request={selectedRequest}
      />

      <LeaveRequestCancelModal
        isOpen={modalType === 'cancel'}
        onClose={handleClose}
        onConfirm={handleCancel}
      />

      <LeaveRequestDeleteModal
        isOpen={modalType === 'delete'}
        onClose={handleClose}
        onConfirm={handleDelete}
      />
    </div>
  );
};

