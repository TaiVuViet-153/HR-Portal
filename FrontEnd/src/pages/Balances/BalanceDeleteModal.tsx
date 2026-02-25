import Modal from '@/components/common/Modal';
import { Trash2 } from 'lucide-react';
import type { SelectedBalance } from './BalancesType';
import { getLeaveTypeKey } from './BalancesData';

type BalanceDeleteModalProps = {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: () => void;
    selectedBalance: SelectedBalance | null;
    error: string | null;
};

export default function BalanceDeleteModal({ isOpen, onClose, onConfirm, selectedBalance, error }: BalanceDeleteModalProps) {
    const userName = selectedBalance?.user?.userName;
    const leaveType = selectedBalance?.balanceItem?.leaveType;
    const year = selectedBalance?.balanceItem?.year;
    const balance = selectedBalance?.balanceItem?.balance;

    return (
        <Modal 
            isOpen={isOpen} 
            onClose={onClose} 
            title="Delete Balance"
            size="sm"
            footer={(
                <>
                    <button 
                        onClick={onClose} 
                        className="px-6 py-2.5 text-sm font-bold text-gray-600 hover:bg-gray-100 rounded-xl transition-all cursor-pointer"
                    >
                        Cancel
                    </button>
                    <button 
                        onClick={onConfirm} 
                        className="px-6 py-2.5 text-sm font-bold bg-red-600 text-white rounded-xl hover:bg-red-700 transition-all shadow-lg shadow-red-100 cursor-pointer"
                    >
                        Delete Balance
                    </button>
                </>
            )}
        >
            <div className="text-center py-4">
                {/* Error Message */}
                {error && (
                    <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-xl text-red-600 text-sm">
                        {error}
                    </div>
                )}

                <div className="w-20 h-20 bg-red-50 text-red-600 rounded-3xl flex items-center justify-center mx-auto mb-6">
                    <Trash2 size={40} />
                </div>
                <h3 className="text-lg font-bold text-gray-900 mb-2">Delete this balance?</h3>
                {selectedBalance ? (
                    <div className="space-y-2">
                        <p className="text-gray-500 font-medium">
                            This will permanently remove the following balance:
                        </p>
                        <div className="bg-gray-50 rounded-xl p-4 mt-4 text-left">
                            <div className="grid grid-cols-2 gap-2 text-sm">
                                <span className="text-gray-500">User:</span>
                                <span className="font-bold text-gray-900">{userName}</span>
                                
                                <span className="text-gray-500">Leave Type:</span>
                                <span className="font-bold text-gray-900">{leaveType !== undefined ? getLeaveTypeKey(leaveType) : 'N/A'}</span>
                                
                                <span className="text-gray-500">Year:</span>
                                <span className="font-bold text-gray-900">{year}</span>
                                
                                <span className="text-gray-500">Balance:</span>
                                <span className="font-bold text-gray-900">{balance} days</span>
                            </div>
                        </div>
                    </div>
                ) : (
                    <p className="text-gray-500 font-medium">
                        This will permanently remove the balance from the system.
                    </p>
                )}
            </div>
        </Modal>
    );
}
