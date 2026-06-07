import { useRef, useEffect, useMemo, useState, useCallback } from 'react'
import { AgGridReact } from 'ag-grid-react'
import 'ag-grid-community/styles/ag-grid.css'
import 'ag-grid-community/styles/ag-theme-quartz.css'
import type {
  ColDef,
  GetRowIdParams,
  RowClassParams,
  RowStyle,
  LocaleText,
} from 'ag-grid-community'
import { theme as antTheme, Pagination, Button, Space } from 'antd'
import { DownloadOutlined, FileExcelOutlined } from '@ant-design/icons'
import { useTranslation } from 'react-i18next'
import * as XLSX from 'xlsx'
import { AG_GRID_LOCALE_HR, AG_GRID_LOCALE_EN } from '@/i18n/agGridLocales'

export interface AgGridWrapperProps<T extends object> {
  columnDefs: ColDef<T>[]
  rowData: T[] | null | undefined
  loading?: boolean
  pageSize?: number
  getRowId?: (params: GetRowIdParams<T>) => string
  getRowStyle?: (params: RowClassParams<T>) => RowStyle | undefined
  isDark: boolean
}

export default function AgGridWrapper<T extends object>({
  columnDefs,
  rowData,
  loading,
  pageSize: defaultPageSize = 20,
  getRowId,
  getRowStyle,
  isDark,
}: AgGridWrapperProps<T>) {
  const gridRef = useRef<AgGridReact<T>>(null)
  const { token } = antTheme.useToken()
  const { i18n } = useTranslation()

  const [currentPage, setCurrentPage] = useState(1)
  const [totalRows, setTotalRows]     = useState(0)
  const [pageSize, setPageSize]       = useState(defaultPageSize)

  const localeText = i18n.language === 'hr' ? AG_GRID_LOCALE_HR : AG_GRID_LOCALE_EN

  useEffect(() => {
    setCurrentPage(1)
    gridRef.current?.api?.paginationGoToPage(0)
  }, [rowData])

  const handlePaginationChanged = useCallback(() => {
    const api = gridRef.current?.api
    if (!api) return
    setCurrentPage(api.paginationGetCurrentPage() + 1)
    setTotalRows(api.paginationGetRowCount())
  }, [])

  const handlePageChange = (page: number, size: number) => {
    const api = gridRef.current?.api
    if (!api) return
    if (size !== pageSize) {
      setPageSize(size)
      api.updateGridOptions({ paginationPageSize: size })
    }
    api.paginationGoToPage(page - 1)
  }

  const handleCsvExport = () => {
    gridRef.current?.api.exportDataAsCsv()
  }

  const handleExcelExport = () => {
    const api = gridRef.current?.api
    if (!api) return
    const csv = api.getDataAsCsv() ?? ''
    const wb = XLSX.read(csv, { type: 'string' })
    XLSX.writeFile(wb, 'izvoz.xlsx')
  }

  const defaultColDef = useMemo<ColDef>(
    () => ({ sortable: true, filter: true, resizable: true }),
    []
  )

  return (
    <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minHeight: 0 }}>

      <div style={{ flex: 1, position: 'relative', minHeight: 0 }}>
        <div
          className="ag-theme-quartz ag-column-separator ag-row-stripes"
          style={{
            position: 'absolute',
            top: 0, right: 0, bottom: 0, left: 0,
            '--ag-active-color': token.colorPrimary,
            '--ag-font-size': '13px',
            '--ag-cell-horizontal-border': `solid 1px ${token.colorBorderSecondary}`,
            '--ag-odd-row-background-color': token.colorFillAlter,
            ...(isDark && {
              '--ag-background-color': '#1f2937',
              '--ag-foreground-color': '#f9fafb',
              '--ag-header-background-color': '#111827',
              '--ag-border-color': '#374151',
              '--ag-row-border-color': '#374151',
              '--ag-secondary-foreground-color': '#f9fafb',
              '--ag-header-foreground-color': '#f9fafb',
              '--ag-odd-row-background-color': '#243044',
            }),
          } as React.CSSProperties}
        >
          <AgGridReact<T>
            ref={gridRef}
            columnDefs={columnDefs}
            rowData={rowData ?? []}
            loading={loading}
            defaultColDef={defaultColDef}
            rowHeight={40}
            pagination
            paginationPageSize={pageSize}
            suppressPaginationPanel
            onPaginationChanged={handlePaginationChanged}
            localeText={localeText as LocaleText}
            getRowId={getRowId}
            getRowStyle={getRowStyle}
            suppressMovableColumns
            animateRows={false}
          />
        </div>
      </div>

      <div style={{
        flexShrink: 0,
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        paddingTop: 12,
        borderTop: `1px solid ${token.colorBorderSecondary}`,
      }}>
        <Space size={6}>
          <Button size="small" icon={<DownloadOutlined />} onClick={handleCsvExport}>
            CSV
          </Button>
          <Button size="small" icon={<FileExcelOutlined />} onClick={handleExcelExport}>
            Excel
          </Button>
        </Space>

        <Pagination
          current={currentPage}
          total={totalRows}
          pageSize={pageSize}
          onChange={handlePageChange}
          showSizeChanger
          pageSizeOptions={[20, 50, 100]}
          showTotal={(total) => i18n.language === 'hr' ? `Ukupno: ${total}` : `Total: ${total}`}
          size="small"
        />
      </div>
    </div>
  )
}
