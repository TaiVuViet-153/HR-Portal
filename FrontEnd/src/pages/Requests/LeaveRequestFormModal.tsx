import Modal from '@/components/common/Modal';
import DatePickerCommon from '@/components/common/DatePickerCommon';
import SelectCommon from '@/components/common/SelectCommon';
import { RequestTypes } from './LeaveRequestsData';
import type { LeaveRequestResponse } from './LeaveRequestsType';
import { useMemo } from 'react';

type LeaveRequestFormModalProps = {
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (e: React.FormEvent) => void;
    mode: 'create' | 'edit';
    request: LeaveRequestResponse | null;
    userName?: string;
    error?: string | null;
};

export default function LeaveRequestFormModal({ 
    isOpen, 
    onClose, 
    onSubmit, 
    mode, 
    request, 
    userName,
    error 
}: LeaveRequestFormModalProps) {
    
    const selectedTypeValue = useMemo(() => {
        if (!request) return 1;
        const idx = RequestTypes[request.type];
        return idx >= 0 ? idx : 0;
    }, [request]);

    return (
        <Modal 
            isOpen={isOpen} 
            onClose={onClose} 
            title={mode === 'create' ? 'Request Time Off' : 'Modify Request'}
            footer={(
                <>
                    <button 
                        onClick={onClose} 
                        className="px-6 py-2.5 text-sm font-bold text-gray-600 hover:bg-gray-100 rounded-xl transition-all cursor-pointer"
                    >
                        Cancel
                    </button>
                    <button 
                        form="leave-form" 
                        type="submit" 
                        className="px-6 py-2.5 text-sm font-bold bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition-all shadow-lg shadow-blue-100 cursor-pointer"
                    >
                        Submit Request
                    </button>
                </>
            )}
        >
            <form
                key={request?.requestId ?? 'new'}
                id="leave-form"
                onSubmit={onSubmit}
                className="grid grid-cols-2 gap-6"
            >
                <div className="col-span-2">
                    <label className="block text-xs font-black text-gray-400 uppercase tracking-widest mb-2">
                        Employee Name
                    </label>
                    <input 
                        name="employeeName" 
                        defaultValue={userName} 
                        required 
                        disabled 
                        className="w-full px-4 py-3 bg-gray-50 border-none rounded-xl focus:ring-2 focus:ring-blue-500" 
                        placeholder="e.g. John Doe" 
                    />
                </div>
                <div>
                    <label className="block text-xs font-black text-gray-400 uppercase tracking-widest mb-2">
                        Start Date
                    </label>
                    <DatePickerCommon name="startDate" defaultValue={request?.startDate} />
                </div>
                <div>
                    <label className="block text-xs font-black text-gray-400 uppercase tracking-widest mb-2">
                        End Date
                    </label>
                    <DatePickerCommon name="endDate" defaultValue={request?.endDate} />
                </div>
                <div className='col-span-2'>
                    <label className="block text-xs font-black text-gray-400 uppercase tracking-widest mb-2">
                        Half Day Off
                    </label>
                    <SelectCommon
                        name="isHalfDayOff"
                        options={[
                            { value: '0', label: 'No' },
                            { value: '1', label: 'Yes' }
                        ]}
                        defaultValue={request?.isHalfDayOff ? '1' : '0'}
                    />
                </div>
                <div className="col-span-2">
                    <label className="block text-xs font-black text-gray-400 uppercase tracking-widest mb-2">
                        Leave Type
                    </label>
                    <SelectCommon
                        name="type"
                        options={Object.entries(RequestTypes).map(([key, value]) => ({ value: String(value), label: key }))}
                        defaultValue={selectedTypeValue}
                        placeholder="Select leave type"
                    />
                </div>
                <div className="col-span-2">
                    <label className="block text-xs font-black text-gray-400 uppercase tracking-widest mb-2">
                        Reason
                    </label>
                    <textarea 
                        name="reason" 
                        rows={3} 
                        defaultValue={request?.reason ?? ''} 
                        required 
                        className="w-full px-4 py-3 bg-gray-50 border-none rounded-xl focus:ring-2 focus:ring-blue-500 resize-none" 
                        placeholder="Explain your request..." 
                    />
                </div>
            </form>
            {error && (<div className="text-red-600 text-sm mt-4">{error}</div>)}
        </Modal>
    );
}
