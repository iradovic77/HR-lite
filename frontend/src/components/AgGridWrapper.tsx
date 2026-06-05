import { useRef, useEffect, useMemo } from 'react'
import { AgGridReact } from 'ag-grid-react'
import {
  themeQuartz,
  colorSchemeDark,
  type ColDef,
  type GetRowIdParams,
  type RowClassParams,
  type RowStyle,
  type CsvExportParams,
  type LocaleText,
} from 'ag-grid-community'
import { theme as antTheme } from 'antd'
import { useTranslation } from 'react-i18next'
import { useTheme } from '@/context/ThemeContext'
import { AG_GRID_LOCALE_HR, AG_GRID_LOCALE_EN } from '@/i18n/agGridLocales'

export interface AgGridWrapperProps<T extends object> {
  columnDefs: ColDef<T>[]
  rowData: T[] | null | undefined
  loading?: boolean
  pageSize?: number
  getRowId?: (params: GetRowIdParams<T>) => string
  getRowStyle?: (params: RowClassParams<T>) => RowStyle | undefined
  exportRef?: React.MutableRefObject<((params?: CsvExportParams) => void) | null>
}

export default function AgGridWrapper<T extends object>({
  columnDefs,
  rowData,
  loading,
  pageSize = 20,
  getRowId,
  getRowStyle,
  exportRef,
}: AgGridWrapperProps<T>) {
  const gridRef = useRef<AgGridReact<T>>(null)
  const { isDark } = useTheme()
  const { token } = antTheme.useToken()
  const { i18n } = useTranslation()

  const gridTheme = useMemo(() => {
    const base = isDark ? themeQuartz.withPart(colorSchemeDark) : themeQuartz
    return base.withParams({
      accentColor: token.colorPrimary,
      rowHeight: 32,
      headerHeight: 36,
      fontSize: 13,
      fontFamily:
        '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif',
    })
  }, [isDark, token.colorPrimary])

  const localeText = i18n.language === 'hr' ? AG_GRID_LOCALE_HR : AG_GRID_LOCALE_EN

  useEffect(() => {
    if (exportRef) {
      exportRef.current = (params) => gridRef.current?.api.exportDataAsCsv(params)
    }
  }, [exportRef])

  const defaultColDef = useMemo<ColDef>(
    () => ({ sortable: true, filter: true, resizable: true }),
    []
  )

  return (
    <div style={{ flex: 1, minHeight: 0, width: '100%', height: '100%' }}>
      <AgGridReact<T>
        ref={gridRef}
        theme={gridTheme}
        columnDefs={columnDefs}
        rowData={rowData ?? []}
        loading={loading}
        defaultColDef={defaultColDef}
        pagination
        paginationPageSize={pageSize}
        paginationPageSizeSelector={[20, 50, 100]}
        localeText={localeText as LocaleText}
        getRowId={getRowId}
        getRowStyle={getRowStyle}
        suppressMovableColumns
        animateRows={false}
      />
    </div>
  )
}
